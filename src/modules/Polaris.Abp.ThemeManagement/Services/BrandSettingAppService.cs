﻿using Microsoft.AspNetCore.Authorization;
using Polaris.Abp.ThemeManagement.Domain;
using Polaris.Abp.ThemeManagement.Permissions;
using Polaris.Abp.ThemeManagement.Services.Dtos;
using Polaris.Abp.ThemeManagement.Services.Interfaces;
using Volo.Abp.Features;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Polaris.Abp.ThemeManagement.Services;

[RequiresFeature(ThemeManagementFeatures.Enable)]
[Authorize(ThemeManagementPermissions.EditBrandSettings)]
public class BrandSettingAppService(ISettingManager settingManager, ISettingManagementStore settingManagementStore) 
    : ThemeManagementAppServiceBase, IBrandSettingAppService
{
    private readonly ISettingManager _settingManager = settingManager;
    private readonly ISettingManagementStore _settingManagementStore = settingManagementStore;

    public async Task<BrandSettingDto> GetAsync()
    {
        var themeSetting = new BrandSettingDto()
        {
            SiteName = await SettingProvider.GetOrNullAsync(ThemeManagementSettings.SiteName),
            LogoUrl = await SettingProvider.GetOrNullAsync(ThemeManagementSettings.LogoUrl),
            LogoReverseUrl = await SettingProvider.GetOrNullAsync(ThemeManagementSettings.LogoReverseUrl)
        };
        return themeSetting;
    }

    public async Task ResetAsync()
    {
        var providerName = TenantSettingValueProvider.ProviderName;
        await _settingManagementStore.DeleteAsync(ThemeManagementSettings.SiteName, providerName, CurrentTenant.Id?.ToString());
        await _settingManagementStore.DeleteAsync(ThemeManagementSettings.LogoUrl, providerName, CurrentTenant.Id?.ToString());
        await _settingManagementStore.DeleteAsync(ThemeManagementSettings.LogoReverseUrl, providerName, CurrentTenant.Id?.ToString());
    }

    public async Task UpdateAsync(BrandSettingDto input)
    {
        await _settingManager.SetForCurrentTenantAsync(ThemeManagementSettings.SiteName, input.SiteName);
        await _settingManager.SetForCurrentTenantAsync(ThemeManagementSettings.LogoUrl, input.LogoUrl);
        await _settingManager.SetForCurrentTenantAsync(ThemeManagementSettings.LogoReverseUrl, input.LogoReverseUrl);
    }
}
