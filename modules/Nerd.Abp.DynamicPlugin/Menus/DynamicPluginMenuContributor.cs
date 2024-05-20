using Nerd.Abp.DynamicPlugin.Localization;
using Nerd.Abp.DynamicPlugin.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace Nerd.Abp.DynamicPlugin.Menus;

public class DynamicPluginMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<DynamicPluginResource>();

        var administrationMenu = context.Menu.GetAdministration();
        //Add main menu items.
        administrationMenu.AddItem(
            new ApplicationMenuItem(DynamicPluginMenus.Prefix,
            displayName: l["Menu:" + DynamicPluginMenus.Prefix],
            "~/DynamicPlugin",
            icon: "fa fa-globe")
             .RequirePermissions(DynamicPluginPermissions.GroupName));

        return Task.CompletedTask;
    }
}
