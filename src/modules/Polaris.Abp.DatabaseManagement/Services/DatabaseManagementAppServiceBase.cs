using Polaris.Abp.DatabaseManagement.Localization;
using Volo.Abp.Application.Services;

namespace Polaris.Abp.DatabaseManagement.Services
{
    public abstract class DatabaseManagementAppServiceBase : ApplicationService
    {
        protected DatabaseManagementAppServiceBase()
        {
            ObjectMapperContext = typeof(DatabaseManagementModule);
            LocalizationResource = typeof(DatabaseManagementResource);
        }
    }
}
