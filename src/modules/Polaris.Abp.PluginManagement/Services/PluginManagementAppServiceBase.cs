using Polaris.Abp.PluginManagement.Localization;
using Volo.Abp.Application.Services;

namespace Polaris.Abp.PluginManagement.Services
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
