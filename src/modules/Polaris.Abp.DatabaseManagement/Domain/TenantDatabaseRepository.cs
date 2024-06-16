using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.DatabaseManagement.Extensions;
using Polaris.Abp.ThemeManagement.Domain;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class TenantDatabaseRepository(IConfigFileManager configFileManager,
    ICurrentTenant currentTenant,
    IServiceProvider serviceProvider) : ITenantDatabaseRepository, ISingletonDependency
{
    private readonly ConcurrentDictionary<Guid, string?> _providerCache = new();
    private readonly IConfigFileManager _configFileManager = configFileManager;
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public string? GetProviderByTenant(Guid? tenantId)
    {
        var key = tenantId.Normalize();
        var databaseProvider = _providerCache.GetOrAdd(key, () => RefreshProviderCache(tenantId).Result);
        return databaseProvider;
    }

#pragma warning disable CRRSP08 // A misspelled word has been found
    public void UpsertProviderForTenant(Guid? tenantId, string? databaseProvider)
#pragma warning restore CRRSP08 // A misspelled word has been found
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
