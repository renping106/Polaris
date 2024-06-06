using Microsoft.AspNetCore.Authorization;
using Nerd.Abp.Extension.Abstractions.Database;
using Nerd.Abp.PluginManagement.Domain;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Permissions;
using Nerd.Abp.PluginManagement.Services.Dtos;
using Nerd.Abp.PluginManagement.Services.Interfaces;
using System.IO.Compression;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.EventBus.Local;

namespace Nerd.Abp.PluginManagement.Services
{
    [Authorize(PluginManagementPermissions.Upload)]
    public class PackageAppService : PluginManagementAppServiceBase, IPackageAppService
    {
        private readonly IBlobContainer _fileContainer;
        private readonly IPlugInManager _plugInManager;
        private readonly IWebAppShell _webAppShell;
        private readonly ILocalEventBus _localEventBus;

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

        public async Task UploadAsync(SaveBlobInputDto input)
        {
            await _fileContainer.SaveAsync(input.Name, input.Content, true);
            await InstallPackageAsync(input.Name);
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
            var pluginFolder = Path.Combine(AppContext.BaseDirectory, "PlugIns", pluginName);
            Directory.Delete(pluginFolder, true);
        }

        private async Task InstallPackageAsync(string name)
        {
            var content = await _fileContainer.GetAllBytesAsync(name);
            using (var stream = new MemoryStream(content))
            {
                using (ZipArchive archive = new ZipArchive(stream))
                {
                    // Locate nuspec
                    var nuspecFile = archive.Entries.FirstOrDefault(e => e.FullName.ToLower().EndsWith(".nuspec"));
                    if (nuspecFile != null)
                    {
                        using StreamReader reader = new(nuspecFile.Open());
                        var packageInfo = XDocument.Load(reader);
                        var pluginName = NuGetUtil.GetMetaValue(packageInfo, "id");
                        var version = NuGetUtil.GetMetaValue(packageInfo, "version");
                        var pluginFolder = Path.Combine(AppContext.BaseDirectory, "PlugIns", pluginName);

                        if (!Directory.Exists(pluginFolder))
                        {
                            Directory.CreateDirectory(pluginFolder);
                            await ExtractPackageAsync(archive, pluginFolder, nuspecFile);
                        }
                        else
                        {
                            var existedPlugin = _plugInManager.GetAllPlugIns().FirstOrDefault(t => t.Name == pluginName);
                            if (existedPlugin != null)
                            {
                                var existedVersion = new Version(existedPlugin.Version);
                                var targetVersion = new Version(version);

                                // Check version
                                if (existedVersion.CompareTo(targetVersion) >= 0)
                                {
                                    throw new UserFriendlyException(L["PluginExists", name]);
                                }

                                // Needs edit permission to update
                                await CheckPolicyAsync(PluginManagementPermissions.Edit);

                                await BackupOldPluginAsync(pluginFolder);

                                await ExtractPackageAsync(archive, pluginFolder, nuspecFile);

                                await ActivePluginAsync(pluginName, pluginFolder, existedPlugin);
                            }
                            else
                            {
                                Directory.Delete(pluginFolder, true);
                                await ExtractPackageAsync(archive, pluginFolder, nuspecFile);
                            }
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException(L["InvalidPlugin"]);
                    }
                }
            }
        }

        private static Task BackupOldPluginAsync(string pluginFolder)
        {
            // Backup old directory
            var pluginFolderBackup = pluginFolder + "_bak";
            if (Directory.Exists(pluginFolderBackup))
            {
                Directory.Delete(pluginFolderBackup, true);
            }

            Directory.Move(pluginFolder, pluginFolderBackup);
            Directory.CreateDirectory(pluginFolder);

            return Task.CompletedTask;
        }

        private async Task ActivePluginAsync(string pluginName, string pluginFolder, IPlugInDescriptor pluginState)
        {
            var pluginFolderBackup = pluginFolder + "_bak";
            try
            {
                if (pluginState.IsEnabled)
                {
                    await UpdatePlugInAsync(pluginName);
                }

                // Remove old directory
                Directory.Delete(pluginFolderBackup, true);
            }
            catch (Exception)
            {
                // Delete extracted files
                Directory.Delete(pluginFolder, true);
                // Rollback old directory
                Directory.Move(pluginFolderBackup, pluginFolder);
                throw;
            }
        }

        private static Task ExtractPackageAsync(ZipArchive archive, string pluginFolder, ZipArchiveEntry nuspec)
        {
            // Extract info file
            ExtractFile(nuspec, pluginFolder, 0);

            // Extract dlls
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                switch (entry.FullName.Split('/')[0])
                {
                    case "lib": // lib/net*/...
                        ExtractFile(entry, pluginFolder, 2);
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private static void ExtractFile(ZipArchiveEntry entry, string folder, int ignoreLeadingSegments)
        {
            string[] segments = entry.FullName.Split('/');
            string filename = Path.Combine(folder, string.Join(Path.DirectorySeparatorChar, segments, ignoreLeadingSegments, segments.Length - ignoreLeadingSegments));

            // Validate path to prevent path traversal
            if (!Path.GetFullPath(filename).StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            entry.ExtractToFile(filename, true);
        }

        private async Task UpdatePlugInAsync(string pluginName)
        {
            var plugin = _plugInManager.GetPlugIn(pluginName);
            var pluginNew = plugin.Clone();
            _plugInManager.DisablePlugIn(plugin);
            var tryAddResult = await _webAppShell.UpdateWebApp();
            if (tryAddResult.Success)
            {          
                await _localEventBus.PublishAsync(new DbContextChangedEvent()
                {
                    DbContextTypes = ((IPlugInContext)plugin.PlugInSource).DbContextTypes
                });

                _plugInManager.EnablePlugIn(pluginNew);
                await _webAppShell.UpdateWebApp();
            }
            else
            {
                throw new UserFriendlyException(L["FailedToUpdatePlugin"]);
            }
        }
    }
}
