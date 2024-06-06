using Nerd.Abp.Extension.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.SettingManagement;
using Nerd.Abp.Extension.Abstractions.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class DbContextChangeHandler : ILocalEventHandler<DbContextChangedEvent>, ITransientDependency
    {
        private readonly IShellEnvironment _shellEnvironment;

        public DbContextChangeHandler(IShellEnvironment shellEnvironment)
        {
            _shellEnvironment = shellEnvironment;
        }

        public async Task HandleEventAsync(DbContextChangedEvent eventData)
        {
            if (_shellEnvironment.ShellServiceProvider == null)
            {
                throw new AbpException("ShellServiceProvider is null.");
            }

            var provider = _shellEnvironment.ShellServiceProvider;
            var migrationManager = provider.GetRequiredService<IMigrationManager>();
            var result = await migrationManager.MigratePluginSchemaAsync(eventData.DbContextTypes);
            if (result > 0)
            {
                var settingManager = provider.GetRequiredService<ISettingManager>();
                var dbVersion = await settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);
                int.TryParse(dbVersion, out int versionNum);
                versionNum++;
                await settingManager.SetGlobalAsync(DatabaseManagementSettings.DatabaseVersion, versionNum.ToString());
            }
        }
    }
}
