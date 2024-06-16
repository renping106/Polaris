using Polaris.Abp.DatabaseManagement.Services.Dtos;
using Volo.Abp;

namespace Polaris.Abp.DatabaseManagement.Services.Interfaces;

public interface ISetupAppService
{
    bool IsInitialized(Guid? tenantId);
    IReadOnlyList<DatabaseProviderDto> GetSupportedDatabaseProviders();
    Task InstallAsync(SetupInputDto input, Guid? tenantId);
    Task<bool> TenantExistsAsync(Guid tenantId);
    Task<List<NameValue>> GetTimezonesAsync();
}
