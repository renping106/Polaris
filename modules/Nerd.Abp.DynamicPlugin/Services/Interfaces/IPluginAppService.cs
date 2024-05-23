using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.DynamicPlugin.Services.Interfaces
{
    public interface IPluginAppService : IApplicationService
    {
        PagedResultDto<PlugInDescriptorDto> GetList();
        Task<PluginStateDto> Enable(string plugInName);
        Task Disable(string plugInName);
    }
}
