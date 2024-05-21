using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.DynamicPlugin.Permissions;
using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Nerd.Abp.DynamicPlugin.Shell;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectMapping;

namespace Nerd.Abp.DynamicPlugin.Services
{
    [Authorize(DynamicPluginPermissions.GroupName)]
    public class PluginAppService : DynamicPluginAppServiceBase, IPluginAppService
    {
        private readonly IPlugInManager _plugInManager;

        public PluginAppService(IPlugInManager plugInManager)
        {
            _plugInManager = plugInManager;
        }

        [Authorize(DynamicPluginPermissions.Edit)]
        public async Task Disable(string plugInName)
        {
            _plugInManager.DisablePlugIn(GetInDescriptor(plugInName));
            await WebAppShell.Instance.UpdateShellAsync();
        }

        [Authorize(DynamicPluginPermissions.Edit)]
        public async Task Enable(string plugInName)
        {
            _plugInManager.EnablePlugIn(GetInDescriptor(plugInName));
            await WebAppShell.Instance.UpdateShellAsync();
        }

        public PagedResultDto<PlugInDescriptorDto> GetList(GetPluginsInputDto input)
        {
            var plugins = _plugInManager.GetAllPlugIns();
            return new PagedResultDto<PlugInDescriptorDto>(
                   plugins.Count,
                   ObjectMapper.Map<IReadOnlyList<IPlugInDescriptor>, List<PlugInDescriptorDto>>(plugins)
                );
        }

        private IPlugInDescriptor GetInDescriptor(string name)
        {
            var plugins = _plugInManager.GetAllPlugIns();
            return plugins.First(p => p.Name == name);
        }
    }
}
