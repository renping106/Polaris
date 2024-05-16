using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DynamicPlugin.Shell;

namespace Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin
{
    public class IndexModel : DynamicPluginPageModel
    {
        public List<string> Plugins { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
            Plugins = WebAppShell.Plugins;
        }

        public async Task<IActionResult> OnPostInstallAsync()
        {
            var (success, message) = await WebAppShell.UpdateShellHostAsync();
            if (!success)
            {
                Message = message;
            }
            else
            {
                Message = "Succeed";
            }
            return RedirectToPage();
        }
    }
}
