using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Volo.Abp.Application.Dtos;

namespace Nerd.Abp.DatabaseManagement.Services.Interfaces
{
    public interface ISetupAppService
    {
        bool IsInitialized(Guid? tenantId);
        IReadOnlyList<DatabaseProviderDto> GetSupportedDatabaseProviders();
    }
}
