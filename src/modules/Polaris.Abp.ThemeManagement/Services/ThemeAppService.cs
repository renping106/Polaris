using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Polaris.Abp.ThemeManagement.Domain;
using Polaris.Abp.ThemeManagement.Permissions;
using Polaris.Abp.ThemeManagement.Services.Dtos;
using Polaris.Abp.ThemeManagement.Services.Interfaces;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.Features;
using Volo.Abp.SettingManagement;

namespace Polaris.Abp.ThemeManagement.Services;

[Authorize]
[RequiresFeature(ThemeManagementFeatures.Enable)]
public class ThemeAppService(IOptions<AbpThemingOptions> options, ISettingManager settingManager) 
    : ThemeManagementAppServiceBase, IThemeAppService
{
    private readonly AbpThemingOptions _options = options.Value;
    private readonly ISettingManager _settingManager = settingManager;

    [Authorize(ThemeManagementPermissions.GroupName)]
    public async Task<PagedResultDto<ThemeDto>> GetThemesAsync()
    {
        var currentTheme = await SettingProvider.GetOrNullAsync(ThemeManagementSettings.ThemeType) ?? "";
        var theme = FindTheme(currentTheme);
        var themes = _options.Themes.ToList();
        return new PagedResultDto<ThemeDto>(
               themes.Count,
               themes.Where(t => t.Key.FullName != null)
               .Select(t => new ThemeDto()
               {
                   Name = t.Value.Name,
                   TypeName = t.Key.FullName!,
                   IsEnabled = theme?.ThemeType.FullName == t.Value.ThemeType.FullName,
               }).ToList()
            );
    }

    [Authorize(ThemeManagementPermissions.Edit)]
    public async Task<bool> EnableAsync(string typeName)
    {
        var target = FindTheme(typeName);
        if (target == null)
        {
            return false;
        }
        else
        {
            await _settingManager.SetForCurrentTenantAsync(ThemeManagementSettings.ThemeType, target.ThemeType.FullName);
            return true;
        }
    }

    private ThemeInfo FindTheme(string typeName)
    {
        var themes = _options.Themes.ToList();
        return themes.Find(t => t.Key.FullName == typeName).Value;
    }
}
