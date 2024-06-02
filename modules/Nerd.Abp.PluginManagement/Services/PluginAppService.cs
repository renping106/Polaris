using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.PluginManagement.Domain;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Permissions;
using Nerd.Abp.PluginManagement.Services.Dtos;
using Nerd.Abp.PluginManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Nerd.Abp.PluginManagement.Services
{
    [Authorize(PluginManagementPermissions.Default)]
    public class PluginAppService : PluginManagementAppServiceBase, IPluginAppService
    {
        private readonly IPlugInManager _plugInManager;
        private readonly IWebAppShell _webAppShell;
        private readonly IPackageAppService _packageAppService;

        public PluginAppService(IPlugInManager plugInManager, IWebAppShell webAppShell, IPackageAppService packageAppService)
        {
            _plugInManager = plugInManager;
            _webAppShell = webAppShell;
            _packageAppService = packageAppService;
        }

        [Authorize(PluginManagementPermissions.Edit)]
        public async Task DisableAsync(string plugInName)
        {
            _plugInManager.DisablePlugIn(GetDescriptor(plugInName));
            await _webAppShell.ResetWebApp();
        }

        [Authorize(PluginManagementPermissions.Edit)]
        public async Task<PluginStateDto> EnableAsync(string plugInName)
        {
            var pluginDescriptor = GetDescriptor(plugInName);
            var folderSource = new FolderSource(((FolderSource)pluginDescriptor.PlugInSource).Folder);
            var testPlugin = new PlugInDescriptor()
            {
                Name = pluginDescriptor.Name,
                Version = pluginDescriptor.Version,
                PlugInSource = folderSource
            };

            var tryAddResult = await _webAppShell.TestWebApp(testPlugin);

            if (tryAddResult.Success)
            {
                _plugInManager.EnablePlugIn(GetDescriptor(plugInName));
                await _webAppShell.ResetWebApp();
            }

            folderSource.Context.Unload(); //unload test context

            return new PluginStateDto()
            {
                Success = tryAddResult.Success,
                Message = tryAddResult.Message
            };
        }

        [Authorize(PluginManagementPermissions.Upload)]
        public void Remove(string plugInName)
        {
            var plugin = GetDescriptor(plugInName);
            if (plugin.IsEnabled)
            {
                throw new UserFriendlyException(L["PluginCannotRemove"]);
            }
            _plugInManager.RemovePlugIn(GetDescriptor(plugInName));
            _packageAppService.RemovePlugIn(plugInName);
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
