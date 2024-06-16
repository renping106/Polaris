using Polaris.Abp.Extension.Abstractions.Database;
using Volo.Abp.DependencyInjection;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class DatabaseProviderFactory(IEnumerable<IDatabaseProvider> providers) : IDatabaseProviderFactory, ITransientDependency
{
    private readonly IEnumerable<IDatabaseProvider> _providers = providers;

    public IDatabaseProvider GetDatabaseProvider(string providerKey)
    {
        return _providers.First(t => t.Key == providerKey);
    }

    public IReadOnlyList<IDatabaseProvider> GetDatabaseProviders()
    {
        return _providers.ToList().AsReadOnly();
    }
}
