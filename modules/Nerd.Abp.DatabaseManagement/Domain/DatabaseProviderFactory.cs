using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class DatabaseProviderFactory : IDatabaseProviderFactory, ITransientDependency
    {
        private readonly IEnumerable<IDatabaseProvider> _providers;

        public DatabaseProviderFactory(IEnumerable<IDatabaseProvider> providers)
        {
            _providers = providers;
        }

        public IDatabaseProvider GetDatabaseProvider(string providerKey)
        {
            return _providers.FirstOrDefault(t => t.Key == providerKey)
                ?? _providers.First(t => t.Key == InMemoryDatabaseProvider.ProviderKey);
        }
    }
}
