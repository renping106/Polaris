using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.UI.Navigation;

namespace Polaris.Abp.NewFireTheme.Themes.NewFire.Components.Menu;

public class MainNavbarMenuViewComponent : AbpViewComponent
{
    private readonly static List<string> _icons = new List<string>()
    {
        "crop",
        "compass",
        "fax",
        "flag",
        "folder",
        "flag",
        "envelope",
        "edit",
        "database",
        "hashtag"
    };

    protected IMenuManager MenuManager { get; }

    public MainNavbarMenuViewComponent(IMenuManager menuManager)
    {
        MenuManager = menuManager;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var menu = await MenuManager.GetMainMenuAsync();
        FillIcons(menu);
        return View("~/Themes/NewFire/Components/Menu/Default.cshtml", menu);
    }

    private void GetAllMenuItems(IHasMenuItems menuWithItems, List<ApplicationMenuItem> output)
    {
        foreach (var item in menuWithItems.Items)
        {
            output.Add(item);
            GetAllMenuItems(item, output);
        }
    }

    private void FillIcons(ApplicationMenu menu)
    {
        var items = new List<ApplicationMenuItem>();
        GetAllMenuItems(menu, items);

        var index = 0;
        foreach (var menuItem in items)
        {
            if (menuItem.Icon == null)
            {
                menuItem.Icon = $"fa fa-{_icons[index]}";
                index++;
            }

            if (index > _icons.Count)
            {
                index = 0;
            }
        }
    }
}
