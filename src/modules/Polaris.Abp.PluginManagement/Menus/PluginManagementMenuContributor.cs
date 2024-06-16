using Polaris.Abp.PluginManagement.Localization;
using Polaris.Abp.PluginManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace Polaris.Abp.PluginManagement.Menus;

public class PluginManagementMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<PluginManagementResource>();

        var administrationMenu = context.Menu.GetAdministration();
        //Add main menu items.
        var group =
            new ApplicationMenuItem(PluginManagementMenus.Prefix, displayName: l["Menu:" + PluginManagementMenus.Prefix], icon: "fa fa-server");
        group.AddItem(
            new ApplicationMenuItem(PluginManagementMenus.List, displayName: l["PlugIns"], "~/PluginManagement").RequirePermissions(PluginManagementPermissions.Default)
            );
        administrationMenu.AddItem(group);

        return Task.CompletedTask;
    }
}
