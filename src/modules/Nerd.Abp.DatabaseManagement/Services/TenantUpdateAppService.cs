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

        public TenantUpdateAppService(ISettingManager settingManager, IDataSeeder dataSeeder)
        {
            _settingManager = settingManager;
            _dataSeeder = dataSeeder;
        }

        public async Task<bool> HasUpdatesAsync()
        {
            if (CurrentTenant.Id.HasValue)
            {
                var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion, false);
                int hostDbVersionNum = 0;
                int.TryParse(hostDbVersion, out hostDbVersionNum);

                var tenantDbVersion = await _settingManager.GetOrNullForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, false);
                int tenantDbVersionNum = 0;
                int.TryParse(tenantDbVersion, out tenantDbVersionNum);

                return tenantDbVersionNum < hostDbVersionNum;
            }
            else
            {
                return false;
            }
        }

        public async Task UpdateDatabaseAsync()
        {
            var migrationManager = LazyServiceProvider.GetRequiredService<IMigrationManager>();
            await migrationManager.MigrateSchemaAsync();

            await _dataSeeder.SeedAsync();

            var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);
            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, hostDbVersion, true);
        }
    }
}
