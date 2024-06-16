using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.Extension.Abstractions.Database;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class CurrentDatabase(ICurrentTenant currentTenant,
    IDatabaseProviderFactory databaseProviderFactory,
    ITenantDatabaseRepository tenantDatabaseRepository) : ICurrentDatabase, ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IDatabaseProviderFactory _databaseProviderFactory = databaseProviderFactory;
    private readonly ITenantDatabaseRepository _tenantDatabaseRepository = tenantDatabaseRepository;

    public IDatabaseProvider Provider => GetCurrent();

    private IDatabaseProvider GetCurrent()
    {
        var providerKey = _tenantDatabaseRepository.GetProviderByTenant(_currentTenant.Id);
        return _databaseProviderFactory.GetDatabaseProvider(providerKey ?? InMemoryDatabaseProvider.ProviderKey);
    }
}
