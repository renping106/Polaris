namespace Polaris.Abp.Extension.Abstractions.Database;

public interface IDatabaseProviderFactory
{
    IDatabaseProvider GetDatabaseProvider(string providerKey);
    IReadOnlyList<IDatabaseProvider> GetDatabaseProviders();
}
