using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.DynamicPlugin.Services
{
    public interface IPluginAppService : IApplicationService
    {
        PagedResultDto<PlugInDescriptorDto> GetList();
        Task Enable(string plugInName);
        Task Disable(string plugInName);
    }
}
