using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.Extension.Abstractions.Database;
using Nerd.Abp.Extension.Abstractions.Plugin;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class DbContextUpdater : IDbContextUpdater, ITransientDependency
    {
        private readonly IShellServiceProvider _shellEnvironment;

        public DbContextUpdater(IShellServiceProvider shellEnvironment)
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
            int.TryParse(dbVersion, out int versionNum);
            versionNum++;
            await settingManager.SetGlobalAsync(DatabaseManagementSettings.DatabaseVersion, versionNum.ToString());
        }
    }
}
