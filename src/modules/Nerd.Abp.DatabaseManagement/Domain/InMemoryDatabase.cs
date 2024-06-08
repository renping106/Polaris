using Microsoft.EntityFrameworkCore;
using Nerd.Abp.Extension.Abstractions.Database;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class InMemoryDatabaseProvider : IDatabaseProvider, ITransientDependency
    {
        public static readonly string ProviderKey = "InMemory";

        public string Name => "In Memory";

        public string Key => ProviderKey;

        public bool HasConnectionString => false;

        public string SampleConnectionString => "InMemory";

        public bool IgnoreMigration => true;

        public UnitOfWorkTransactionBehavior UnitOfWorkTransactionBehaviorOption => UnitOfWorkTransactionBehavior.Disabled;

        public SequentialGuidType? SequentialGuidTypeOption => null;

        public DbContextOptionsBuilder UseDatabase(AbpDbContextConfigurationContext context)
        {
            var connectionString = context.ConnectionString;
            if (connectionString.IsNullOrEmpty()) { connectionString = $"Temp{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}"; }
            return context.DbContextOptions.UseInMemoryDatabase(connectionString);
        }
    }
}
