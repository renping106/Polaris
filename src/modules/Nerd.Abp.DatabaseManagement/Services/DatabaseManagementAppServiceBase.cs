using Nerd.Abp.DatabaseManagement.Localization;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.DatabaseManagement.Services
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
