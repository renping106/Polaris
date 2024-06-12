using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Polaris.Abp.NewFireTheme.Bundling;

public class PolarisNewFireThemeGlobalStyleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add("/themes/newfire/layout.css");
        context.Files.Add("/themes/newfire/bootstrap/select2-bootstrap-5-theme.css");
        context.Files.Add("/themes/newfire/bootstrap/select2-bootstrap-5-theme-rtl.css");
    }
}
