using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace Polaris.Abp.NewFireTheme;

[ThemeName(Name)]
internal class PolarisNewFireTheme : ITheme, ITransientDependency
{
    public const string Name = "NewFire";

    public virtual string GetLayout(string name, bool fallbackToDefault = true)
    {
        return name switch
        {
            StandardLayouts.Public => "~/Themes/NewFire/Layouts/Public.cshtml",
            StandardLayouts.Application => "~/Themes/NewFire/Layouts/Application.cshtml",
            StandardLayouts.Account => "~/Themes/NewFire/Layouts/Account.cshtml",
            StandardLayouts.Empty => "~/Themes/NewFire/Layouts/Empty.cshtml",
            _ => fallbackToDefault ? "~/Themes/NewFire/Layouts/Public.cshtml" : "",
        };
    }
}
