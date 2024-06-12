using Polaris.Abp.Host.Localization;
using Volo.Abp.Application.Services;

namespace Polaris.Abp.Host.Services;

/* Inherit your application services from this class. */
public abstract class HostAppService : ApplicationService
{
    protected HostAppService()
    {
        LocalizationResource = typeof(HostResource);
    }
}