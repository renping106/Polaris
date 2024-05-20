using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DynamicPlugin.Permissions;
using Nerd.Abp.DynamicPlugin.Shell;
using Volo.Abp;

namespace Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin
{
    public class IndexModel : DynamicPluginPageModel
    {
        public List<IPlugInDescriptor> Plugins { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool HasEditPermission { get; set; }

        private readonly IPlugInManager _plugInManager;

        public IndexModel(IPlugInManager plugInManager)
        {
            _plugInManager = plugInManager;
            Plugins =  _plugInManager.GetAllPlugIns().ToList();
            HasEditPermission = AuthorizationService.IsGrantedAsync(DynamicPluginPermissions.Edit).GetAwaiter().GetResult();
        }

        public async void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostEnableAsync(string name)
        {
            var target = Plugins.Find(x => x.Name == name);
            if (target != null)
            {
                _plugInManager.EnablePlugIn(target);
            }

            var (success, message) = await WebAppShell.Instance.UpdateShellAsync();
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

        public async Task<IActionResult> OnPostDisableAsync(string name)
        {
            var target = Plugins.Find(x => x.Name == name);
            if (target != null)
            {
                _plugInManager.DisablePlugIn(target);
            }

            var (success, message) = await WebAppShell.Instance.UpdateShellAsync();
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
