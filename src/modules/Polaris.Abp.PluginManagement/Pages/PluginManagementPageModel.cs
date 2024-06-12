using Polaris.Abp.PluginManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Polaris.Abp.PluginManagement.Pages
{
    public abstract class PluginManagementPageModel : AbpPageModel
    {
        protected PluginManagementPageModel()
        {
            LocalizationResourceType = typeof(PluginManagementResource);
            ObjectMapperContext = typeof(PluginManagementModule);
        }
    }
}
