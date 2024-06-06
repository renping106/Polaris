using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Permissions;
using Nerd.Abp.PluginManagement.Services.Dtos;
using Nerd.Abp.PluginManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;

namespace Nerd.Abp.PluginManagement.Services
{
    [Authorize(PluginManagementPermissions.Default)]
    public class PluginAppService : PluginManagementAppServiceBase, IPluginAppService
    {
        private readonly IPlugInManager _plugInManager;
        private readonly IWebAppShell _webAppShell;
        private readonly IPackageAppService _packageAppService;
        private readonly ILocalEventBus _localEventBus;

        public PluginAppService(
            IPlugInManager plugInManager,
            IWebAppShell webAppShell,
            IPackageAppService packageAppService,
            ILocalEventBus localEventBus)
        {
            _plugInManager = plugInManager;
            _webAppShell = webAppShell;
            _packageAppService = packageAppService;
            _localEventBus = localEventBus;
        }

        [Authorize(PluginManagementPermissions.Edit)]
        public async Task DisableAsync(string plugInName)
        {
            _plugInManager.DisablePlugIn(_plugInManager.GetPlugIn(plugInName));
            await _webAppShell.UpdateWebApp();
        }

        [Authorize(PluginManagementPermissions.Edit)]
        public async Task<PluginStateDto> EnableAsync(string plugInName)
        {
            var pluginDescriptor = _plugInManager.GetPlugIn(plugInName);
            var targetPlugIn = pluginDescriptor.Clone();

            _plugInManager.SetPreEnabledPlugIn(targetPlugIn);

            var tryAddResult = await _webAppShell.UpdateWebApp();

            if (!tryAddResult.Success)
            {
                ((IPlugInContext)targetPlugIn.PlugInSource).UnloadContext();
            }

            return new PluginStateDto()
            {
                Success = tryAddResult.Success,
                Message = tryAddResult.Message
            };
        }

        /// <summary>
        /// Need a second call to use latest DI container for dbcontexts retrieving
        /// </summary>
        /// <param name="plugInName"></param>
        /// <returns></returns>
        [Authorize(PluginManagementPermissions.Edit)]
        public async Task EnableCommitAsync(string plugInName)
        {
            var targetPlugIn = _plugInManager.GetEnabledPlugIns().First(t => t.Name == plugInName);
            await _localEventBus.PublishAsync(new DbContextChangedEvent()
            {
                DbContextTypes = ((IPlugInContext)targetPlugIn.PlugInSource).DbContextTypes
            });
            _plugInManager.EnablePlugIn(targetPlugIn);
        }

        [Authorize(PluginManagementPermissions.Upload)]
        public void Remove(string plugInName)
        {
            var plugin = _plugInManager.GetPlugIn(plugInName);
            if (plugin.IsEnabled)
            {
                throw new UserFriendlyException(L["PluginCannotRemove"]);
            }
            _plugInManager.RemovePlugIn(_plugInManager.GetPlugIn(plugInName));
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
    }
}
