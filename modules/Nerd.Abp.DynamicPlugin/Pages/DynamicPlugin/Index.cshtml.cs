using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DynamicPlugin.Shell;

namespace Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin
{
    public class IndexModel : DynamicPluginPageModel
    {
        public List<string> Plugins { get; set; } = new List<string>();

        public void OnGet()
        {
            Plugins = WebAppShell.Plugins;
        }

        public async Task<IActionResult> OnPostInstallAsync()
        {
            await WebAppShell.UpdateShellHost(HttpContext);
            return RedirectToPage();
        }
    }
}
