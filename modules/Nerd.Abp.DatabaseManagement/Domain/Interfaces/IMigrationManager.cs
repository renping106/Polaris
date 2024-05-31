namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface IMigrationManager
    {
        Task MigrateSchemaAsync();
    }
}
