using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Polaris.Abp.ThemeManagement.Domain;
using Polaris.Abp.ThemeManagement.Localization;
using Polaris.Abp.ThemeManagement.Permissions;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace Polaris.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup;

public class PolarisThemeSettingPageContributor : SettingPageContributorBase
{
    public PolarisThemeSettingPageContributor()
    {
        RequiredTenantSideFeatures(ThemeManagementFeatures.Enable);
        RequiredPermissions(ThemeManagementPermissions.EditBrandSettings);
    }

    public override Task ConfigureAsync(SettingPageCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<ThemeManagementResource>>();
        context.Groups.Add(
            new SettingPageGroup(
                "Polaris.Abp.ThemeManagement.BrandSetting",
                l["Menu:BrandSetting"],
                typeof(PolarisThemeSettingViewComponent)
            )
        );
        return Task.CompletedTask;
    }
}
