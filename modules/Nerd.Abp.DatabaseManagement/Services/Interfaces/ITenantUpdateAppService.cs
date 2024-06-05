namespace Nerd.Abp.DatabaseManagement.Services.Interfaces
{
    public interface ITenantUpdateAppService
    {
        Task<bool> HasUpdatesAsync();
        Task UpdateDatabaseAsync();
    }
}
