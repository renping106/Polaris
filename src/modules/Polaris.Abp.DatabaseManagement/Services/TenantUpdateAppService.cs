using Polaris.Abp.DatabaseManagement.Domain;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.DatabaseManagement.Extensions;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;
using Polaris.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Polaris.Abp.DatabaseManagement.Services;

[RemoteService(false)]
public class TenantUpdateAppService : DatabaseManagementAppServiceBase, ITenantUpdateAppService, ITransientDependency
{
    private readonly ICurrentDatabase _currentDatabase;
    private readonly IDistributedCache<DbVersionCache> _dbVersionCacheForInMemory;
    private readonly object _locker = new object();
    private readonly IDatabaseMigrationService _migrationService;
    private readonly ISettingManager _settingManager;

    public TenantUpdateAppService(
        ISettingManager settingManager,
        ICurrentDatabase currentDatabase,
        IDatabaseMigrationService migrationService,
        IDistributedCache<DbVersionCache> dbVersionCache)
    {
        _settingManager = settingManager;
        _currentDatabase = currentDatabase;
        _migrationService = migrationService;
        _dbVersionCacheForInMemory = dbVersionCache;
    }

    public async Task<bool> HasUpdatesAsync()
    {
        if (CurrentTenant.Id.HasValue)
        {
            var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion, false);
            int.TryParse(hostDbVersion, out var hostDbVersionNum);

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
            var cacheItem = _dbVersionCacheForInMemory.GetOrAdd(CurrentTenant.Id.Normalize().ToString(), () =>
            {
                return new DbVersionCache(-1);
            }) ?? new DbVersionCache(-1);

            return cacheItem.DatabaseVersion;
        }

        var tenantDbVersion = await _settingManager.GetOrNullForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, false);
        int.TryParse(tenantDbVersion, out var tenantDbVersionNum);
        return tenantDbVersionNum;
    }

    private async Task SyncTenantDbVersionAsync()
    {
        var hostDbVersion = await _settingManager.GetOrNullGlobalAsync(DatabaseManagementSettings.DatabaseVersion);

        if (_currentDatabase.Provider.Key == InMemoryDatabaseProvider.ProviderKey)
        {
            _dbVersionCacheForInMemory.Set(CurrentTenant.Id.Normalize().ToString(), new DbVersionCache(int.Parse(hostDbVersion)));
            return;
        }

        await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseVersion, hostDbVersion, true);
    }

    public record DbVersionCache(int DatabaseVersion);
}
