using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class CurrentDatabase : ICurrentDatabase, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IDatabaseProviderFactory _databaseProviderFactory;
        private readonly ITenantDatabaseRepository _tenantDatabaseRepository;

        public CurrentDatabase(ICurrentTenant currentTenant,
            IDatabaseProviderFactory databaseProviderFactory,
            ITenantDatabaseRepository tenantDatabaseRepository)
        {
            _currentTenant = currentTenant;
            _databaseProviderFactory = databaseProviderFactory;
            _tenantDatabaseRepository = tenantDatabaseRepository;
        }

        public IDatabaseProvider Provider => GetCurrent();

        private IDatabaseProvider GetCurrent()
        {
            var providerKey = _tenantDatabaseRepository.GetProviderByTenant(_currentTenant.Id);
            return _databaseProviderFactory.GetDatabaseProvider(providerKey ?? InMemoryDatabaseProvider.ProviderKey);
        }
    }
}
