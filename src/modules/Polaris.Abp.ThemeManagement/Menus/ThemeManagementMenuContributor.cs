using Polaris.Abp.ThemeManagement.Domain;
using Polaris.Abp.ThemeManagement.Localization;
using Polaris.Abp.ThemeManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.UI.Navigation;

namespace Polaris.Abp.ThemeManagement.Menus;

internal class ThemeManagementMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<ThemeManagementResource>();

        var administrationMenu = context.Menu.GetAdministration();
        //Add main menu items.
        var group =
            new ApplicationMenuItem(ThemeManagementMenus.Prefix, displayName: l["Menu:" + ThemeManagementMenus.Prefix], icon: "fa fa-camera");
        group.AddItem(
            new ApplicationMenuItem(ThemeManagementMenus.List, displayName: l["Themes"], "~/ThemeManagement")
            .RequirePermissions(ThemeManagementPermissions.GroupName)
            .RequireFeatures(ThemeManagementFeatures.Enable)
            );
        administrationMenu.AddItem(group);

        return Task.CompletedTask;
    }
}