using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Nerd.Abp.DynamicPlugin.Shell;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.DynamicPlugin.Services
{
    public interface IPluginAppService : IApplicationService
    {
        PagedResultDto<PlugInDescriptorDto> GetList(GetPluginsInputDto input);
        Task Enable(string plugInName);
        Task Disable(string plugInName);
    }
}
