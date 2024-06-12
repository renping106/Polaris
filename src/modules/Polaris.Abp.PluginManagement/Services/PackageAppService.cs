using Microsoft.AspNetCore.Authorization;
using Polaris.Abp.PluginManagement.Domain;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Polaris.Abp.PluginManagement.Permissions;
using Polaris.Abp.PluginManagement.Services.Dtos;
using Polaris.Abp.PluginManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.BlobStoring;

namespace Polaris.Abp.PluginManagement.Services
{
    [Authorize(PluginManagementPermissions.Upload)]
    public class PackageAppService : PluginManagementAppServiceBase, IPackageAppService
    {
        private readonly IBlobContainer _fileContainer;
        private readonly IPlugInManager _plugInManager;

        public PackageAppService(IBlobContainer fileContainer, IPlugInManager plugInManager)
        {
            _fileContainer = fileContainer;
            _plugInManager = plugInManager;
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
                await _plugInManager.RefreshPlugInAsync(installedPlugin);
            }
        }
    }
}
