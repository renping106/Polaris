namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface IDatabaseProviderFactory
    {
        IDatabaseProvider GetDatabaseProvider(string providerKey);
        IReadOnlyList<IDatabaseProvider> GetDatabaseProviders();
    }
}
