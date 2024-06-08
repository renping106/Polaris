using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.Extension.Abstractions.Database;
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
        private readonly IPackageAppService _packageAppService;
        private readonly IPlugInManager _plugInManager;

        public PluginAppService(
            IPlugInManager plugInManager,
            IWebAppShell webAppShell,
            IPackageAppService packageAppService,
            IDbContextUpdater dbContextUpdator)
        {
            _plugInManager = plugInManager;
            _packageAppService = packageAppService;
        }

        [Authorize(PluginManagementPermissions.Edit)]
        public async Task DisableAsync(string plugInName)
        {
            await _plugInManager.DisablePlugInAsync(plugInName);
        }

        [Authorize(PluginManagementPermissions.Edit)]
        public async Task<PluginStateDto> EnableAsync(string plugInName)
        {
            var tryAddResult = await _plugInManager.EnablePlugInAsync(plugInName);
            return new PluginStateDto()
            {
                Success = tryAddResult.Success,
                Message = tryAddResult.Message
            };
        }

        public PagedResultDto<PlugInDescriptorDto> GetList()
        {
            var plugins = _plugInManager.GetAllPlugIns(true);
            return new PagedResultDto<PlugInDescriptorDto>(
                   plugins.Count,
                   ObjectMapper.Map<IReadOnlyList<IPlugInDescriptor>, List<PlugInDescriptorDto>>(plugins)
                );
        }

        [Authorize(PluginManagementPermissions.Upload)]
        public void Remove(string plugInName)
        {
            var plugin = _plugInManager.GetPlugIn(plugInName);
            if (plugin.IsEnabled)
            {
                throw new UserFriendlyException(L["PluginCannotRemove"]);
            }
            _plugInManager.RemovePlugIn(plugInName);
            _packageAppService.RemovePlugIn(plugInName);
        }
    }
}
