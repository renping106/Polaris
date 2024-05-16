using Nerd.Abp.DynamicPlugin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Nerd.Abp.DynamicPlugin.Pages
{
    public abstract class DynamicPluginPageModel : AbpPageModel
    {
        protected DynamicPluginPageModel()
        {
            LocalizationResourceType = typeof(DynamicPluginResource);
            ObjectMapperContext = typeof(DynamicPluginModule);
        }
    }
}
