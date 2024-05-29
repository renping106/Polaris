using Nerd.Abp.DatabaseManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Nerd.Abp.DatabaseManagement.Pages
{
    public abstract class DatabaseManagementPageModel : AbpPageModel
    {
        protected DatabaseManagementPageModel()
        {
            LocalizationResourceType = typeof(DatabaseManagementResource);
        }
    }

}
