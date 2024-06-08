using Microsoft.EntityFrameworkCore;
using Nerd.Abp.Extension.Abstractions.Database;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Nerd.Abp.DatabaseManagement.Sqlite
{
    public class SqliteDatabaseProvider : IDatabaseProvider, ITransientDependency
    {
        public static readonly string ProviderKey = "Sqlite";

        public string Name => "Sqlite";

        public string Key => ProviderKey;

        public bool HasConnectionString => true;

        public string SampleConnectionString => "Data Source=Nerd.db;Cache=Shared";

        public bool IgnoreMigration => false;

        public UnitOfWorkTransactionBehavior UnitOfWorkTransactionBehaviorOption => UnitOfWorkTransactionBehavior.Disabled;

        public SequentialGuidType? SequentialGuidTypeOption => null;

        public DbContextOptionsBuilder UseDatabase(AbpDbContextConfigurationContext context)
        {
            return context.UseSqlite();
        }
    }
}
