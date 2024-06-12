using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Polaris.Abp.NewFireTheme.Themes.NewFire.Components.MainNavbar;

public class MainNavbarViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Themes/NewFire/Components/MainNavbar/Default.cshtml");
    }
}
