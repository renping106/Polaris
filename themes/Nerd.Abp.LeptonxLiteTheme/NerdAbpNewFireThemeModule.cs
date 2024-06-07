using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Modularity;

namespace Nerd.Abp.NewFireTheme
{
    [DependsOn(
    typeof(AbpAspNetCoreMvcUiThemeSharedModule)
    )]
    public class NerdAbpNewFireThemeModule : AbpModule
    {

    }
}
