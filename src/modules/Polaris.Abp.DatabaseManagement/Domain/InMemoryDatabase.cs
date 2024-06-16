using Microsoft.EntityFrameworkCore;
using Polaris.Abp.Extension.Abstractions.Database;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class InMemoryDatabaseProvider(IClock clock) : IDatabaseProvider, ITransientDependency
{
    public readonly static string ProviderKey = "InMemory";

    public string Name => "In Memory";

    public string Key => ProviderKey;

    public bool HasConnectionString => false;

    public string SampleConnectionString => $"Temp{_clock.Now:yyyyMMddHHmmss}";

    public bool IgnoreMigration => true;

    public UnitOfWorkTransactionBehavior UnitOfWorkTransactionBehaviorOption => UnitOfWorkTransactionBehavior.Disabled;

    public SequentialGuidType? SequentialGuidTypeOption => null;

    private readonly IClock _clock = clock;

    public DbContextOptionsBuilder UseDatabase(AbpDbContextConfigurationContext context)
    {
        var connectionString = context.ConnectionString;
        if (connectionString.IsNullOrEmpty()) { connectionString = $"Temp{_clock.Now:yyyyMMddHHmmss}"; }
        return context.DbContextOptions.UseInMemoryDatabase(connectionString);
    }
}
