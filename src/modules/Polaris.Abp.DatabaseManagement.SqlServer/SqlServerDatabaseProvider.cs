using Microsoft.EntityFrameworkCore;
using Polaris.Abp.Extension.Abstractions.Database;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.ConnectionStrings;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.DatabaseManagement.SqlServer
{
    public class SqlServerDatabaseProvider : IDatabaseProvider, ITransientDependency
    {
        public virtual string Name => "SQL Server";

        public virtual string Key => "SqlServer";

        public virtual bool HasConnectionString => true;

        public virtual string SampleConnectionString => "Server=localhost;Database=Polaris;User Id=username;Password=password";

        public bool IgnoreMigration => false;

        public UnitOfWorkTransactionBehavior UnitOfWorkTransactionBehaviorOption => UnitOfWorkTransactionBehavior.Auto;

        public SequentialGuidType? SequentialGuidTypeOption => SequentialGuidType.SequentialAtEnd;

        public DbContextOptionsBuilder UseDatabase(AbpDbContextConfigurationContext context)
        {
            return context.UseSqlServer();
        }
    }
}
