using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.DatabaseManagement.Services
{
    [RemoteService(false)]
    public class TenantUpdateAppService : DatabaseManagementAppServiceBase, ITenantUpdateAppService, ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly IDataSeeder _dataSeeder;
        private readonly ICurrentDatabase _currentDatabase;

        public TenantUpdateAppService(ISettingManager settingManager, IDataSeeder dataSeeder, ICurrentDatabase currentDatabase)
        {
            _settingManager = settingManager;
            _dataSeeder = dataSeeder;
            _currentDatabase = currentDatabase;
        }

        public async Task<bool> HasUpdatesAsync()
        {
            if (CurrentTenant.Id.HasValue)
            {
                var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion, false);
                int.TryParse(hostDbVersion, out int hostDbVersionNum);

                var tenantDbVersion = await _settingManager.GetOrNullForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, false);
                int.TryParse(tenantDbVersion, out int tenantDbVersionNum);

                return tenantDbVersionNum < hostDbVersionNum;
            }
            else
            {
                return false;
            }
        }

        public async Task UpdateDatabaseAsync()
        {
            if (!_currentDatabase.Provider.IgnoreMigration)
            {
                var migrationManager = LazyServiceProvider.GetRequiredService<IMigrationManager>();
                await migrationManager.MigrateSchemaAsync();
            }

            await _dataSeeder.SeedAsync(CurrentTenant.Id);

            var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);
            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, hostDbVersion, true);
        }
    }
}
