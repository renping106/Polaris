using Nerd.Abp.Extension.Abstractions.Database;
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
            return _providers.First(t => t.Key == providerKey);
        }

        public IReadOnlyList<IDatabaseProvider> GetDatabaseProviders()
        {
            return _providers.ToList().AsReadOnly();
        }
    }
}
