using Microsoft.EntityFrameworkCore;
using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.ConnectionStrings;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.SqlServer
{
    public class SqlServerDatabaseProvider : IDatabaseProvider, ITransientDependency
    {
        public virtual string Name => "SQL Server";

        public virtual string Key => "SqlServer";

        public virtual bool HasConnectionString => true;

        public virtual string SampleConnectionString => "Server=localhost;Database=Nerd;User Id=username;Password=password";

        public bool IgnoreMigration => false;

        public UnitOfWorkTransactionBehavior UnitOfWorkTransactionBehaviorOption => UnitOfWorkTransactionBehavior.Auto;

        public SequentialGuidType? SequentialGuidTypeOption => SequentialGuidType.SequentialAtEnd;

        public async Task<AbpConnectionStringCheckResult> CheckConnectionString(string connectionString)
        {
            var checker = new SqlServerConnectionStringChecker();
            var result = await checker.CheckAsync(connectionString);
            return result;
        }

        public DbContextOptionsBuilder UseDatabase(AbpDbContextConfigurationContext context)
        {
            return context.UseSqlServer();
        }
    }
}
