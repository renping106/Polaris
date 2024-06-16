using Polaris.Abp.PluginManagement;
using Polaris.Abp.ThemeManagement.Localization;
using Volo.Abp.Application.Services;

namespace Polaris.Abp.ThemeManagement.Services;

public abstract class ThemeManagementAppServiceBase : ApplicationService
{
    protected ThemeManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(ThemeManagementModule);
        LocalizationResource = typeof(ThemeManagementResource);
    }
}
