using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.Extension.Abstractions.Database;
using Polaris.Abp.Extension.Abstractions.Plugin;
using Polaris.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class HostDbContextUpdater : IDbContextUpdater, ITransientDependency
{
    private readonly IShellServiceProvider _shellEnvironment;

    public HostDbContextUpdater(IShellServiceProvider shellEnvironment)
    {
        _shellEnvironment = shellEnvironment;
    }

    public async Task UpdateAsync(DbContextChangedEvent dbContextChangedEvent)
    {
        if (_shellEnvironment.ServiceProvider == null)
        {
            throw new AbpException("ShellServiceProvider is null.");
        }

        var provider = _shellEnvironment.ServiceProvider;
        var migrationManager = provider.GetRequiredService<IMigrationManager>();
        await migrationManager.MigratePluginSchemaAsync(dbContextChangedEvent.DbContextTypes);

        var dataSeeder = provider.GetRequiredService<IDataSeeder>();
        await dataSeeder.SeedAsync();

        var settingManager = provider.GetRequiredService<ISettingManager>();
        var dbVersion = await settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);
        int.TryParse(dbVersion, out var versionNum);
        versionNum++;
        await settingManager.SetGlobalAsync(DatabaseManagementSettings.DatabaseVersion, versionNum.ToString());
    }
}
