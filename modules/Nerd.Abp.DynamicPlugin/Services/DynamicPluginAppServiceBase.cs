using Nerd.Abp.DynamicPlugin.Localization;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.DynamicPlugin.Services
{
    public abstract class DynamicPluginAppServiceBase : ApplicationService
    {
        protected DynamicPluginAppServiceBase()
        {
            ObjectMapperContext = typeof(DynamicPluginModule);
            LocalizationResource = typeof(DynamicPluginResource);
        }
    }
}
