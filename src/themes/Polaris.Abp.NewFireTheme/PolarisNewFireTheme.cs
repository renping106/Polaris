using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace Polaris.Abp.NewFireTheme
{
    [ThemeName(Name)]
    internal class PolarisNewFireTheme : ITheme, ITransientDependency
    {
        public const string Name = "NewFire";

        public virtual string GetLayout(string name, bool fallbackToDefault = true)
        {
            switch (name)
            {
                case StandardLayouts.Public:
                    return "~/Themes/NewFire/Layouts/Public.cshtml";
                case StandardLayouts.Application:
                    return "~/Themes/NewFire/Layouts/Application.cshtml";
                case StandardLayouts.Account:
                    return "~/Themes/NewFire/Layouts/Account.cshtml";
                case StandardLayouts.Empty:
                    return "~/Themes/NewFire/Layouts/Empty.cshtml";
                default:
                    return fallbackToDefault ? "~/Themes/NewFire/Layouts/Public.cshtml" : null;
            }
        }
    }
}
