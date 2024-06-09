using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.ThemeManagement.Domain;
using Nerd.Abp.ThemeManagement.Permissions;
using Nerd.Abp.ThemeManagement.Services.Dtos;
using Nerd.Abp.ThemeManagement.Services.Interfaces;
using Volo.Abp.Features;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.ThemeManagement.Services
{
    [RequiresFeature(ThemeManagementFeatures.Enable)]
    [Authorize(ThemeManagementPermissions.EditBrandSettings)]
    public class BrandSettingAppService : ThemeManagementAppServiceBase, IBrandSettingAppService
    {
        private readonly ISettingManager _settingManager;

        public BrandSettingAppService(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public async Task<ThemeSettingDto> GetAsync()
        {
            var themeSetting = new ThemeSettingDto()
            {
                LogoUrl = await SettingProvider.GetOrNullAsync(ThemeManagementSettings.LogoUrl),
                LogoReverseUrl = await SettingProvider.GetOrNullAsync(ThemeManagementSettings.LogoReverseUrl)
            };
            return themeSetting;
        }

        public async Task UpdateAsync(ThemeSettingDto input)
        {
            await _settingManager.SetForCurrentTenantAsync(ThemeManagementSettings.LogoUrl, input.LogoUrl);
            await _settingManager.SetForCurrentTenantAsync(ThemeManagementSettings.LogoReverseUrl, input.LogoReverseUrl);
        }
    }
}
