using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Reflection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;
using Nerd.Abp.DatabaseManagement.Data;
using Nerd.Abp.DatabaseManagement.Domain.Entities;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.Extension.Abstractions.Database;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class MigrationManager : IMigrationManager, ITransientDependency
    {
        public static readonly string MigrationNamespace = "Nerd.Abp.EntityFrameworkCore.Migrations";
        public static readonly string MigrationClassName = "NerdDbContextModelSnapshot";

        private readonly IClock _clock;
        private readonly IEnumerable<IAbpEfCoreDbContext> _dbContexts;
        private readonly ICurrentDatabase _currentDatabase;
        private readonly DatabaseManagementDbContext _modelHistoryContext;
        private readonly IDbContextLocator _dbContextLocator;
        private readonly List<string> _pendingChanges = new List<string>();
        private readonly List<ModelHistory> _snapshots = new List<ModelHistory>();
        private bool _flag = true;

        public MigrationManager(IClock clock,
            DatabaseManagementDbContext modelHistoryContext,
            IDbContextsResolver dbContextsResolver,
            ICurrentDatabase currentDatabase,
            IDbContextLocator dbContextLocator)
        {
            _dbContexts = dbContextsResolver.DbContexts;
            _clock = clock;
            _currentDatabase = currentDatabase;
            _modelHistoryContext = modelHistoryContext;
            _dbContextLocator = dbContextLocator;
        }

        public async Task MigrateSchemaAsync()
        {
            foreach (var dbContext in _dbContexts)
            {
                if (!dbContext.Database.CanConnect())
                {
                    var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();
                    await dbCreator.CreateAsync();
                }

                MigrateDatabase(dbContext);
            }

            CommitChanges();
        }

        public Task<int> MigratePluginSchemaAsync(IReadOnlyList<Type> pluginDbContextTypes)
        {
            var pluginDbContextNames = pluginDbContextTypes.Select(x => x.FullName).ToList();
            foreach (var dbContext in _dbContexts.Where(t => pluginDbContextNames.Contains(t.GetType().FullName)))
            {
                MigrateDatabase(dbContext);
            }

            CommitChanges();

            return Task.FromResult(_pendingChanges.Count);
        }

        private void MigrateDatabase(IAbpEfCoreDbContext dbContext)
        {
            var lastModel = GetLastModelSnapshot(dbContext);

            var modelDiffer = dbContext.Database.GetService<IMigrationsModelDiffer>();
            var sourceModel = lastModel?.GetRelationalModel();
            var targetModel = dbContext.GetService<IDesignTimeModel>().Model.GetRelationalModel();
            if (modelDiffer.HasDifferences(sourceModel, targetModel))
            {
                var upOperations = modelDiffer.GetDifferences(sourceModel, targetModel);

                // Generate SQL
                GeneratePendingChanges(dbContext, upOperations);

                // Generate new snapshot
                GenerateSnapshot(dbContext);
            }
        }

        private IModel? GetLastModelSnapshot(IAbpEfCoreDbContext dbContext)
        {
            IModel? lastModel = null;
            try
            {
                // Read latest snapshot
                // TODO Find a better way that not throwing exceptions
                ModelHistory? lastSnapshot = null;
                if (_modelHistoryContext.Database.CanConnect())
                {
                    try
                    {
                        if (_flag)
                        {
                            lastSnapshot = _modelHistoryContext.Set<ModelHistory>()
                                                               .Where(t => t.DbContextFullName == dbContext.GetType().FullName)
                                                               .OrderByDescending(e => e.Id)
                                                               .FirstOrDefault();
                        }
                    }
                    catch (Exception)
                    {
                        //No snapshot table
                        _flag = false;
                    }
                }

                lastModel = lastSnapshot == null
                    ? null
                    : (CreateModelSnapshot(dbContext, lastSnapshot.Snapshot, MigrationNamespace, MigrationClassName)?.Model);
            }
            catch (DbException)
            {
                // Take failure as null
            }

            if (lastModel is IMutableModel mutableModel)
            {
                lastModel = mutableModel.FinalizeModel();
            }

            if (lastModel != null)
            {
                lastModel = dbContext.GetService<IModelRuntimeInitializer>().Initialize(lastModel);
            }

            return lastModel;
        }

        private void GenerateSnapshot(IAbpEfCoreDbContext dbContext)
        {
            var snapshotCode = ModelSnapshotToString(dbContext, MigrationNamespace, MigrationClassName);
            _snapshots.Add(new ModelHistory()
            {
                DbContextFullName = dbContext.GetType().FullName ?? "Unknow",
                Snapshot = snapshotCode,
                SnapshotTime = _clock.Now
            });
        }

        private void GeneratePendingChanges(IAbpEfCoreDbContext dbContext, IReadOnlyList<MigrationOperation> upOperations)
        {
            var sqlList = dbContext.Database.GetService<IMigrationsSqlGenerator>()
                .Generate(upOperations, ((DbContext)dbContext).Model)
                .Select(p => p.CommandText).ToList();

            foreach (var item in sqlList)
            {
                _pendingChanges.AddIfNotContains(item);
            }
        }

        private void CommitChanges()
        {
            if (_pendingChanges.Count > 0)
            {
                _modelHistoryContext.ExecuteListSqlCommand(_pendingChanges);
                _modelHistoryContext.Set<ModelHistory>().AddRange(_snapshots);
                _modelHistoryContext.SaveChanges();
            }
            _pendingChanges.Clear();
            _snapshots.Clear();
        }

        private static string ModelSnapshotToString(IAbpEfCoreDbContext dbContext, string nameSpace, string className)
        {
            var snapshotCode = new DesignTimeServicesBuilder(
                                     dbContext.GetType().Assembly,
                                     Assembly.GetEntryAssembly(),
                                     new OperationReporter(new OperationReportHandler()), Array.Empty<string>())
                    .Build((DbContext)dbContext)
                    .GetRequiredService<IMigrationsCodeGenerator>()
                    .GenerateSnapshot(nameSpace, dbContext.GetType(), className, dbContext.GetService<IDesignTimeModel>().Model);
            return snapshotCode;
        }

        private ModelSnapshot? CreateModelSnapshot(IAbpEfCoreDbContext dbContext, string codedefine, string nameSpace, string className)
        {
            var dbProvider = _currentDatabase.Provider;
            var providerReferences = dbProvider.GetType().Assembly.GetReferencedAssemblies()
                .Select(e => MetadataReference.CreateFromFile(Assembly.Load(e).Location));

            var references = dbContext.GetType().Assembly
                .GetReferencedAssemblies()
                .Select(e => MetadataReference.CreateFromFile(_dbContextLocator.GetReferenceLocation(dbContext, e)))
                .Union(new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Abstractions").Location),
                    MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
                    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
                    MetadataReference.CreateFromFile(_dbContextLocator.GetLocation(dbContext))
                })
                .Union(providerReferences);
            var compilation = CSharpCompilation.Create(nameSpace)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(codedefine));

            using var stream = new MemoryStream();
            var compileResult = compilation.Emit(stream);
            return compileResult.Success
                ? Assembly.Load(stream.GetBuffer()).CreateInstance(nameSpace + "." + className) as ModelSnapshot
                : null;
        }
    }
}
