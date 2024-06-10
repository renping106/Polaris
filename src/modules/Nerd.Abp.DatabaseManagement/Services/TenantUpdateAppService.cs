using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Extensions;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.DatabaseManagement.Services
{
    [RemoteService(false)]
    public class TenantUpdateAppService : DatabaseManagementAppServiceBase, ITenantUpdateAppService, ITransientDependency
    {
        private readonly ICurrentDatabase _currentDatabase;
        private readonly IDataSeeder _dataSeeder;
        private readonly IDistributedCache<DbVersionCache> _dbVersionCache;
        private readonly ISettingManager _settingManager;

        public TenantUpdateAppService(
            ISettingManager settingManager,
            IDataSeeder dataSeeder,
            ICurrentDatabase currentDatabase,
            IDistributedCache<DbVersionCache> dbVersionCache)
        {
            _settingManager = settingManager;
            _dataSeeder = dataSeeder;
            _currentDatabase = currentDatabase;
            _dbVersionCache = dbVersionCache;
        }

        public async Task<bool> HasUpdatesAsync()
        {
            if (CurrentTenant.Id.HasValue)
            {
                var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion, false);
                int.TryParse(hostDbVersion, out int hostDbVersionNum);

                var tenantDbVersionNum = await GetTenantDbVersionAsync();

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

            await SyncTenantDbVersionAsync();
        }

        private async Task<int> GetTenantDbVersionAsync()
        {
            if (_currentDatabase.Provider.IgnoreMigration)
            {
                var cacheItem = InitCache();
                return cacheItem.DatabaseVersion;
            }

            var tenantDbVersion = await _settingManager.GetOrNullForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, false);
            int.TryParse(tenantDbVersion, out int tenantDbVersionNum);
            return tenantDbVersionNum;
        }

        private DbVersionCache InitCache()
        {
            return _dbVersionCache.GetOrAdd(CurrentTenant.Id.Normalize().ToString(), () =>
            {
                return new DbVersionCache();
            }) ?? new DbVersionCache();
        }

        private async Task SyncTenantDbVersionAsync()
        {
            var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);

            if (_currentDatabase.Provider.IgnoreMigration)
            {
                var cacheItem = InitCache();
                cacheItem.DatabaseVersion = int.Parse(hostDbVersion);
                _dbVersionCache.Set(CurrentTenant.Id.Normalize().ToString(), cacheItem);
                return;
            }

            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, hostDbVersion, true);
        }

        public class DbVersionCache
        {
            public int DatabaseVersion { get; set; }
        }
    }
}
