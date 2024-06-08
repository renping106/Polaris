using Nerd.Abp.PluginManagement;
using Nerd.Abp.ThemeManagement.Localization;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.ThemeManagement.Services
{
    public abstract class ThemeManagementAppServiceBase : ApplicationService
    {
        protected ThemeManagementAppServiceBase()
        {
            ObjectMapperContext = typeof(ThemeManagementModule);
            LocalizationResource = typeof(ThemeManagementResource);
        }
    }
}
