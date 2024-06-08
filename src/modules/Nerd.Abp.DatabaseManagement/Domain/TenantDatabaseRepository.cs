using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Extensions;
using Nerd.Abp.ThemeManagement.Domain;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class TenantDatabaseRepository : ITenantDatabaseRepository, ISingletonDependency
    {
        private readonly ConcurrentDictionary<Guid, string?> _providerCache;
        private readonly IConfigFileManager _configFileManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceProvider _serviceProvider;

        public TenantDatabaseRepository(IConfigFileManager configFileManager,
            ICurrentTenant currentTenant,
            IServiceProvider serviceProvider)
        {
            _providerCache = new ConcurrentDictionary<Guid, string?>();
            _configFileManager = configFileManager;
            _currentTenant = currentTenant;
            _serviceProvider = serviceProvider;
        }

        public string? GetProviderByTenant(Guid? tenantId)
        {
            var key = tenantId.Normalize();
            var databaseProvider = _providerCache.GetOrAdd(key, () => RefreshProviderCache(tenantId).Result);
            return databaseProvider;
        }

        public void UpsertProviderForTenant(Guid? tenantId, string? databaseProvider)
        {
            var key = tenantId.Normalize();
            _providerCache.AddOrUpdate(key, (_) => databaseProvider, (_, _) => databaseProvider);
        }

        private async Task<string?> RefreshProviderCache(Guid? tenantId)
        {
            var databaseProvider = string.Empty;
            if (!tenantId.HasValue)
            {
                databaseProvider = _configFileManager.GetDatabaseProvider();
            }
            else
            {
                using (_currentTenant.Change(null))
                {
                    //TODO try settingmanager
                    var settingRepo = _serviceProvider.GetRequiredService<ISettingStore>();
                    var tenantDatabaseProvider =
                        await settingRepo.GetOrNullAsync(DatabaseManagementSettings.DatabaseProvider, 
                            TenantSettingValueProvider.ProviderName, tenantId.Value.ToString());

                    databaseProvider = tenantDatabaseProvider;
                }
            }

            return databaseProvider;
        }
    }
}
