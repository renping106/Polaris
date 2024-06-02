using Nerd.Abp.DatabaseManagement.Services.Dtos;

namespace Nerd.Abp.DatabaseManagement.Services.Interfaces
{
    public interface ISetupAppService
    {
        bool IsInitialized(Guid? tenantId);
        IReadOnlyList<DatabaseProviderDto> GetSupportedDatabaseProviders();
        Task InstallAsync(SetupInputDto input, Guid? tenantId);
    }
}
