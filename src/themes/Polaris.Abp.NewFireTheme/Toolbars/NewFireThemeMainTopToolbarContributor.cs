using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.NewFireTheme.Themes.NewFire.Components.Toolbar.LanguageSwitch;
using Polaris.Abp.NewFireTheme.Themes.NewFire.Components.Toolbar.UserMenu;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Localization;
using Volo.Abp.Users;

namespace Polaris.Abp.NewFireTheme.Toolbars;

public class NewFireThemeMainTopToolbarContributor : IToolbarContributor
{
    public async Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
    {
        if (context.Toolbar.Name != StandardToolbars.Main)
        {
            return;
        }

        if (context.Theme is not PolarisNewFireTheme)
        {
            return;
        }

        var languageProvider = context.ServiceProvider.GetService<ILanguageProvider>();

        if (languageProvider != null)
        {
            var languages = await languageProvider.GetLanguagesAsync();
            if (languages.Count > 1)
            {
                context.Toolbar.Items.Add(new ToolbarItem(typeof(LanguageSwitchViewComponent)));
            }

            if (context.ServiceProvider.GetRequiredService<ICurrentUser>().IsAuthenticated)
            {
                context.Toolbar.Items.Add(new ToolbarItem(typeof(UserMenuViewComponent)));
            }
        }
    }
}