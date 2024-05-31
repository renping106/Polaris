namespace Nerd.Abp.DatabaseManagement.Abstractions.Database
{
    public interface IDatabaseProviderFactory
    {
        IDatabaseProvider GetDatabaseProvider(string providerKey);
        IReadOnlyList<IDatabaseProvider> GetDatabaseProviders();
    }
}
