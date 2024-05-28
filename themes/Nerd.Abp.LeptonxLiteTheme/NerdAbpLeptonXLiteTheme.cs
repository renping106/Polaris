using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace Nerd.Abp.LeptonxLiteTheme
{
    [ThemeName(Name)]
    public class NerdAbpLeptonXLiteTheme : LeptonXLiteTheme, ITransientDependency
    {
        public new const string Name = "AbpLeptonXLite";
    }
}
