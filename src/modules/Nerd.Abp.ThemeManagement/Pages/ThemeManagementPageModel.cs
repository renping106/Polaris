using Nerd.Abp.ThemeManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Nerd.Abp.ThemeManagement.Pages
{
    public abstract class ThemeManagementPageModel : AbpPageModel
    {
        protected ThemeManagementPageModel()
        {
            LocalizationResourceType = typeof(ThemeManagementResource);
            ObjectMapperContext = typeof(ThemeManagementPageModel);
        }
    }
}
