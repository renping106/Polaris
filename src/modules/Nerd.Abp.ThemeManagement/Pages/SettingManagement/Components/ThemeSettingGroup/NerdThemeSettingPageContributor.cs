using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nerd.Abp.ThemeManagement.Domain;
using Nerd.Abp.ThemeManagement.Localization;
using Nerd.Abp.ThemeManagement.Permissions;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

namespace Nerd.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup
{
    public class NerdThemeSettingPageContributor : SettingPageContributorBase
    {
        public NerdThemeSettingPageContributor()
        {
            RequiredTenantSideFeatures(ThemeManagementFeatures.Enable);
            RequiredPermissions(ThemeManagementPermissions.EditBrandSettings);
        }

        public override Task ConfigureAsync(SettingPageCreationContext context)
        {
            var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<ThemeManagementResource>>();
            context.Groups.Add(
                new SettingPageGroup(
                    "Nerd.Abp.ThemeManagement.BrandSetting",
                    l["Menu:BrandSetting"],
                    typeof(NerdThemeSettingViewComponent)
                )
            );
            return Task.CompletedTask;
        }
    }
}
