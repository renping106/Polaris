using Nerd.Abp.DatabaseManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

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
