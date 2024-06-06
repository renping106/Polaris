using Nerd.Abp.Extension.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class DbContextChangeHandler : ILocalEventHandler<DbContextChangedEvent>, ITransientDependency
    {
        private readonly IMigrationManager _migrationManager;
        private readonly ISettingManager _settingManager;

        public DbContextChangeHandler(IMigrationManager migrationManager, ISettingManager settingManager)
        {
            _migrationManager = migrationManager;
            _settingManager = settingManager;
        }

        public async Task HandleEventAsync(DbContextChangedEvent eventData)
        {
            var result = await _migrationManager.MigratePluginSchemaAsync(eventData.DbContextTypes);
            if (result > 0)
            {
                var dbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);
                int versionNum = 0;
                int.TryParse(dbVersion, out versionNum);
                versionNum++;
                await _settingManager.SetGlobalAsync(DatabaseManagementSettings.DatabaseVersion, versionNum.ToString());
            }
        }
    }
}
