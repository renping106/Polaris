using Microsoft.AspNetCore.Authorization;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Polaris.Abp.PluginManagement.Permissions;
using Polaris.Abp.PluginManagement.Services.Dtos;
using Polaris.Abp.PluginManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Polaris.Abp.PluginManagement.Services;

[Authorize(PluginManagementPermissions.Default)]
public class PluginAppService(IPlugInManager plugInManager, IPackageAppService packageAppService) 
    : PluginManagementAppServiceBase, IPluginAppService
{
    private readonly IPackageAppService _packageAppService = packageAppService;
    private readonly IPlugInManager _plugInManager = plugInManager;

    [Authorize(PluginManagementPermissions.Edit)]
    public async Task DisableAsync(string plugInName)
    {
        await _plugInManager.DisablePlugInAsync(plugInName);
    }

    [Authorize(PluginManagementPermissions.Edit)]
    public async Task<PluginStateDto> EnableAsync(string plugInName)
    {
        var (Success, Message) = await _plugInManager.EnablePlugInAsync(plugInName);
        return new PluginStateDto()
        {
            Success = Success,
            Message = Message
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
