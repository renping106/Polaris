using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.Extension.Abstractions.Database;
using Nerd.Abp.PluginManagement.Domain;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Permissions;
using Nerd.Abp.PluginManagement.Services.Dtos;
using Nerd.Abp.PluginManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.EventBus.Local;

namespace Nerd.Abp.PluginManagement.Services
{
    [Authorize(PluginManagementPermissions.Upload)]
    public class PackageAppService : PluginManagementAppServiceBase, IPackageAppService
    {
        private readonly IBlobContainer _fileContainer;
        private readonly ILocalEventBus _localEventBus;
        private readonly IPlugInManager _plugInManager;
        private readonly IWebAppShell _webAppShell;

        public PackageAppService(
            IBlobContainer fileContainer,
            IPlugInManager plugInManager,
            IWebAppShell webAppShell,
            ILocalEventBus localEventBus)
        {
            _fileContainer = fileContainer;
            _plugInManager = plugInManager;
            _webAppShell = webAppShell;
            _localEventBus = localEventBus;
        }

        public async Task<BlobDto> GetAsync(GetBlobRequestDto input)
        {
            var blob = await _fileContainer.GetAllBytesAsync(input.Name);

            return new BlobDto
            {
                Name = input.Name,
                Content = blob
            };
        }

        public void RemovePlugIn(string pluginName)
        {
            PlugInPackageUtil.RemovePackage(pluginName);
        }

        public async Task UploadAsync(SaveBlobInputDto input)
        {
            await _fileContainer.SaveAsync(input.Name, input.Content, true);
            await InstallPackageAsync(input.Name);
        }

        private async Task InstallPackageAsync(string packageName)
        {
            var content = await _fileContainer.GetAllBytesAsync(packageName);
            var descriptor = PlugInPackageUtil.LoadPackage(content);

            if (descriptor == null)
            {
                throw new UserFriendlyException(L["InvalidPlugin"]);
            }

            var installedPlugin = _plugInManager.GetAllPlugIns().FirstOrDefault(t => t.Name == descriptor.Name);

            if (installedPlugin != null)
            {
                var installedVersion = new Version(installedPlugin.Version);
                var targetVersion = new Version(descriptor.Version);

                // Check version
                if (installedVersion.CompareTo(targetVersion) >= 0)
                {
                    throw new UserFriendlyException(L["PluginExists", descriptor.Name]);
                }
            }

            PlugInPackageUtil.InstallPackage(descriptor, content);

            if (installedPlugin != null && installedPlugin.IsEnabled)
            {
                await RefreshPluginAsync(installedPlugin);
            }
        }

        private async Task RefreshPluginAsync(IPlugInDescriptor installedPlugIn)
        {
            try
            {
                ((IPlugInContext)installedPlugIn.PlugInSource).UnloadContext();
                var tryAddResult = await _webAppShell.UpdateShell();

                if (tryAddResult.Success)
                {
                    await _localEventBus.PublishAsync(new DbContextChangedEvent()
                    {
                        DbContextTypes = ((IPlugInContext)installedPlugIn.PlugInSource).DbContextTypes
                    });
                }
                else
                {
                    throw new AbpException(tryAddResult.Message);
                }
            }
            catch (Exception)
            {
                PlugInPackageUtil.RollbackPackage(installedPlugIn);

                // Rollback shell
                ((IPlugInContext)installedPlugIn.PlugInSource).UnloadContext();
                await _webAppShell.UpdateShell();
                throw;
            }
        }
    }
}
