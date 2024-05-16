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
        //Add main menu items.
        context.Menu.AddItem(
            new ApplicationMenuItem(DynamicPluginMenus.Prefix, 
            displayName: DynamicPluginMenus.Prefix, 
            "~/DynamicPlugin", 
            icon: "fa fa-globe")
             .RequirePermissions(DynamicPluginPermissions.List));

        return Task.CompletedTask;
    }
}
