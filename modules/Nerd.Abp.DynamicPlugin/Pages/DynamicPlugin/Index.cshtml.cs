using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DynamicPlugin.Shell;
using Volo.Abp;

namespace Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin
{
    public class IndexModel : DynamicPluginPageModel
    {
        public List<string> Plugins { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
        private readonly IAbpApplication _abpApplication;

        public IndexModel(IAbpApplication abpApplication)
        {
            _abpApplication = abpApplication;
        }

        public void OnGet()
        {
            Plugins = _abpApplication.Modules.Where(t => t.IsLoadedAsPlugIn)
                .Select(t => t.Assembly.FullName ?? "")
                .ToList();
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
