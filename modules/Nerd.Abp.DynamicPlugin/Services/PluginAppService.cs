using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.DynamicPlugin.Domain;
using Nerd.Abp.DynamicPlugin.Domain.Interfaces;
using Nerd.Abp.DynamicPlugin.Permissions;
using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Nerd.Abp.DynamicPlugin.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

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
            _plugInManager.DisablePlugIn(GetDescriptor(plugInName));
            await WebAppShell.Instance.UpdateShellAsync();
        }

        [Authorize(DynamicPluginPermissions.Edit)]
        public async Task<PluginStateDto> Enable(string plugInName)
        {
            var pluginDescriptor = GetDescriptor(plugInName);
            var tryAddResult = await WebAppShell.Instance.TestShellAsync(pluginDescriptor);

            if (tryAddResult.Success)
            {
                _plugInManager.EnablePlugIn(GetDescriptor(plugInName));
                await WebAppShell.Instance.UpdateShellAsync();
            }
            return new PluginStateDto()
            {
                Success = tryAddResult.Success,
                Message = tryAddResult.Message
            };
        }

        [Authorize(DynamicPluginPermissions.Edit)]
        public async Task Remove(string plugInName)
        {
            var plugin = GetDescriptor(plugInName);
            if (plugin.IsEnabled)
            {
                throw new UserFriendlyException("Cannot remove an active plugin");
            }
            _plugInManager.RemovePlugIn(GetDescriptor(plugInName));
        }

        public PagedResultDto<PlugInDescriptorDto> GetList()
        {
            var plugins = _plugInManager.GetAllPlugIns(true);
            return new PagedResultDto<PlugInDescriptorDto>(
                   plugins.Count,
                   ObjectMapper.Map<IReadOnlyList<IPlugInDescriptor>, List<PlugInDescriptorDto>>(plugins)
                );
        }

        private IPlugInDescriptor GetDescriptor(string name)
        {
            var plugins = _plugInManager.GetAllPlugIns();
            return plugins.First(p => p.Name == name);
        }
    }
}
