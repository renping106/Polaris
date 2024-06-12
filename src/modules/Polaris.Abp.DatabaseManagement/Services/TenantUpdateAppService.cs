using Polaris.Abp.DatabaseManagement.Domain;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.DatabaseManagement.Extensions;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;
using Polaris.Abp.ThemeManagement.Domain;
using System.Collections.Concurrent;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Polaris.Abp.DatabaseManagement.Services
{
    [RemoteService(false)]
    public class TenantUpdateAppService : DatabaseManagementAppServiceBase, ITenantUpdateAppService, ITransientDependency
    {
        private static readonly ConcurrentDictionary<string, DbVersionCache> _dbVersionCache
            = new ConcurrentDictionary<string, DbVersionCache>();
        private readonly ICurrentDatabase _currentDatabase;
        private readonly object _locker = new object();
        private readonly ISettingManager _settingManager;
        private readonly IDatabaseMigrationService _migrationService;

        public TenantUpdateAppService(
            ISettingManager settingManager,
            ICurrentDatabase currentDatabase,
            IDatabaseMigrationService migrationService)
        {
            _settingManager = settingManager;
            _currentDatabase = currentDatabase;
            _migrationService = migrationService;
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

        public Task UpdateDatabaseAsync()
        {
            lock (_locker)
            {
                if (HasUpdatesAsync().GetAwaiter().GetResult())
                {
                    if (_currentDatabase.Provider.Key == InMemoryDatabaseProvider.ProviderKey)
                    {
                        var email = _settingManager.GetOrNullForCurrentTenantAsync(DatabaseManagementSettings.DefaultAdminEmail).GetAwaiter().GetResult();
                        var password = _settingManager.GetOrNullForCurrentTenantAsync(DatabaseManagementSettings.DefaultAdminPassword).GetAwaiter().GetResult();
                        _migrationService.MigrateAsync(email, password);
                    }
                    else
                    {
                        _migrationService.MigrateAsync();
                    }

                    SyncTenantDbVersionAsync().GetAwaiter().GetResult();
                }
            }

            return Task.CompletedTask;
        }

        private async Task<int> GetTenantDbVersionAsync()
        {
            if (_currentDatabase.Provider.Key == InMemoryDatabaseProvider.ProviderKey)
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
            });
        }

        private async Task SyncTenantDbVersionAsync()
        {
            var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);

            if (_currentDatabase.Provider.Key == InMemoryDatabaseProvider.ProviderKey)
            {
                var cacheItem = InitCache();
                cacheItem.DatabaseVersion = int.Parse(hostDbVersion);
                return;
            }

            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, hostDbVersion, true);
        }

        public class DbVersionCache
        {
            public int DatabaseVersion { get; set; } = -1;
        }
    }
}
