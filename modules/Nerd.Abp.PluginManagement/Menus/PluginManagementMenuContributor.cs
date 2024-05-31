using Nerd.Abp.PluginManagement.Localization;
using Nerd.Abp.PluginManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace Nerd.Abp.PluginManagement.Menus;

public class PluginManagementMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<PluginManagementResource>();

        var administrationMenu = context.Menu.GetAdministration();
        //Add main menu items.
        administrationMenu.AddItem(
            new ApplicationMenuItem(PluginManagementMenus.Prefix,
            displayName: l["Menu:" + PluginManagementMenus.Prefix],
            "~/PluginManagement",
            icon: "fa fa-server")
             .RequirePermissions(PluginManagementPermissions.GroupName));

        return Task.CompletedTask;
    }
}
