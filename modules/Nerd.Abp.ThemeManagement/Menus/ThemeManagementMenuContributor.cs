using Nerd.Abp.ThemeManagement.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Nerd.Abp.ThemeManagement.Permissions;

namespace Nerd.Abp.ThemeManagement.Menus
{
    internal class ThemeManagementMenuContributor : IMenuContributor
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
            var l = context.GetLocalizer<ThemeManagementResource>();

            var administrationMenu = context.Menu.GetAdministration();
            //Add main menu items.
            administrationMenu.AddItem(
                new ApplicationMenuItem(ThemeManagementMenus.Prefix,
                displayName: l["Menu:" + ThemeManagementMenus.Prefix],
                "~/ThemeManagement",
                icon: "fa fa-globe")
                 .RequirePermissions(ThemeManagementPermissions.GroupName));

            return Task.CompletedTask;
        }
    }
}