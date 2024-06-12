using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Polaris.Abp.NewFireTheme.Themes.NewFire.Components.Brand;

public class MainNavbarBrandViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Themes/NewFire/Components/Brand/Default.cshtml");
    }
}
