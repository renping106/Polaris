using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface IMigrationManager
    {
        Task MigrateSchemaAsync();
        Task MigratePluginSchemaAsync(Type pluginDbContextType);
    }
}
