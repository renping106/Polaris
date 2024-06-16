namespace Polaris.Abp.DatabaseManagement.Domain.Interfaces;

public interface IMigrationManager
{
    Task MigrateSchemaAsync();
    Task<int> MigratePluginSchemaAsync(IReadOnlyList<Type> pluginDbContextTypes);
}
