using Polaris.Abp.PluginManagement.Services.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Polaris.Abp.PluginManagement.Services.Interfaces;

public interface IPluginAppService : IApplicationService
{
    PagedResultDto<PlugInDescriptorDto> GetList();
    Task<PluginStateDto> EnableAsync(string plugInName);
    Task DisableAsync(string plugInName);
    void Remove(string plugInName);
}
