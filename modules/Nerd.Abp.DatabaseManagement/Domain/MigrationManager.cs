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

        public MigrationManager(IClock clock,
            DatabaseManagementDbContext modelHistoryContext,
            IDbContextsResolver dbContextsResolver,
            ICurrentDatabase currentDatabase)
        {
            _dbContexts = dbContextsResolver.DbContexts;
            _clock = clock;
            _currentDatabase = currentDatabase;
            _modelHistoryContext = modelHistoryContext;
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

                Migrate(dbContext, upOperations);

                // generate new migration and save it
                SaveSnapshot(dbContext);
            }
        }

        private IModel? GetLastModelSnapshot(IAbpEfCoreDbContext dbContext)
        {
            IModel? lastModel = null;
            try
            {
                // read latest snapshot
                ModelHistory? lastSnapshot = null;
                if (_modelHistoryContext.Database.CanConnect())
                {
                    try
                    {
                        lastSnapshot = _modelHistoryContext.Set<ModelHistory>()
                                                 .Where(t => t.DbContextFullName == dbContext.GetType().FullName)
                                                 .OrderByDescending(e => e.Id)
                                                 .FirstOrDefault();
                    }
                    catch (Exception)
                    {
                    }
                }

                lastModel = lastSnapshot == null
                    ? null
                    : (CreateModelSnapshot(dbContext, lastSnapshot.Snapshot, MigrationNamespace, MigrationClassName)?.Model);
            }
            catch (DbException) { }

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

        private void SaveSnapshot(IAbpEfCoreDbContext dbContext)
        {
            var snapshotCode = ModelSnapshotToString(dbContext, MigrationNamespace, MigrationClassName);

            _modelHistoryContext.Set<ModelHistory>().Add(new ModelHistory()
            {
                DbContextFullName = dbContext.GetType().FullName,
                Snapshot = snapshotCode,
                SnapshotTime = _clock.Now
            });
            _modelHistoryContext.SaveChanges();
        }

        private static void Migrate(IAbpEfCoreDbContext dbContext, IReadOnlyList<MigrationOperation> upOperations)
        {
            var sqlList = dbContext.Database.GetService<IMigrationsSqlGenerator>()
                .Generate(upOperations, ((DbContext)dbContext).Model)
                .Select(p => p.CommandText).ToList();
            int changeCount = dbContext.ExecuteListSqlCommand(sqlList);
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
                .Select(e => MetadataReference.CreateFromFile(Assembly.Load(e).Location))
                .Union(new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Abstractions").Location),
                    MetadataReference.CreateFromFile(Assembly.Load("Microsoft.EntityFrameworkCore.Relational").Location),
                    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
                    MetadataReference.CreateFromFile(dbContext.GetType().Assembly.Location)
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
