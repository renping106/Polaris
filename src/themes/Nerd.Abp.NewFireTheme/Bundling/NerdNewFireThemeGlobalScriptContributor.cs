using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Nerd.Abp.NewFireTheme.Bundling;

public class NerdNewFireThemeGlobalScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add("/themes/newfire/layout.js");

        context.Files.ReplaceOne(
            "/libs/abp/aspnetcore-mvc-ui-theme-shared/bootstrap/dom-event-handlers.js",
            "/themes/newfire/bootstrap/dom-event-handlers.js"
        );
    }
}
