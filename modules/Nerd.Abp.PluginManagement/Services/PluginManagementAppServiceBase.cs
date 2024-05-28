using Nerd.Abp.PluginManagement.Localization;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.PluginManagement.Services
{
    public abstract class PluginManagementAppServiceBase : ApplicationService
    {
        protected PluginManagementAppServiceBase()
        {
            ObjectMapperContext = typeof(PluginManagementModule);
            LocalizationResource = typeof(PluginManagementResource);
        }
    }
}
