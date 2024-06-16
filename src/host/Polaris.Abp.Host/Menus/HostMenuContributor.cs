using Polaris.Abp.Host.Localization;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Polaris.Abp.Host.Menus;

public class HostMenuContributor : IMenuContributor
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
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<HostResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                HostMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);

        return Task.CompletedTask;
    }
}
