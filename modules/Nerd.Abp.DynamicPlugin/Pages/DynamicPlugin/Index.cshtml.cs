using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DynamicPlugin.Permissions;
using Nerd.Abp.DynamicPlugin.Shell;

namespace Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin
{
    public class IndexModel : DynamicPluginPageModel
    {
        public List<IPlugInDescriptor> Plugins { get; set; }
        public bool HasEditPermission { get; set; }

        private readonly IPlugInManager _plugInManager;

        public IndexModel(IPlugInManager plugInManager)
        {
            _plugInManager = plugInManager;
            Plugins =  _plugInManager.GetAllPlugIns().ToList();

        }

        public async void OnGet()
        {
            HasEditPermission = await AuthorizationService.IsGrantedAsync(DynamicPluginPermissions.Edit);
        }

        public async Task<IActionResult> OnPostEnableAsync(string name)
        {
            var target = Plugins.Find(x => x.Name == name);
            if (target != null)
            {
                _plugInManager.EnablePlugIn(target);
            }

            await WebAppShell.Instance.UpdateShellAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDisableAsync(string name)
        {
            var target = Plugins.Find(x => x.Name == name);
            if (target != null)
            {
                _plugInManager.DisablePlugIn(target);
            }

            await WebAppShell.Instance.UpdateShellAsync();
            return RedirectToPage();
        }
    }
}
