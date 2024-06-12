namespace Polaris.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface ITenantDatabaseRepository
    {
        string? GetProviderByTenant(Guid? tenantId);
        void UpsertProviderForTenant(Guid? tenantId, string? databaseProvider);
    }
}
