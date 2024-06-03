using Nerd.Abp.PluginManagement.Services.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.PluginManagement.Services.Interfaces
{
    public interface IPluginAppService : IApplicationService
    {
        PagedResultDto<PlugInDescriptorDto> GetList();
        Task<PluginStateDto> EnableAsync(string plugInName);
        Task MigrateSchema();
        Task DisableAsync(string plugInName);
        void Remove(string plugInName);
    }
}
