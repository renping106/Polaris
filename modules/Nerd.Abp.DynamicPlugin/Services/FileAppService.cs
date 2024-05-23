using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Nerd.Abp.DynamicPlugin.Services.Interfaces;
using System.IO.Compression;
using Volo.Abp.BlobStoring;

namespace Nerd.Abp.DynamicPlugin.Services
{
    public class FileAppService : DynamicPluginAppServiceBase, IFileAppService
    {
        private readonly IBlobContainer _fileContainer;

        public FileAppService(IBlobContainer fileContainer)
        {
            _fileContainer = fileContainer;
        }

        public async Task SaveBlobAsync(SaveBlobInputDto input)
        {
            await _fileContainer.SaveAsync(input.Name, input.Content, true);
            await InstallPackageAsync(input.Name);
        }

        public async Task<BlobDto> GetBlobAsync(GetBlobRequestDto input)
        {
            var blob = await _fileContainer.GetAllBytesAsync(input.Name);

            return new BlobDto
            {
                Name = input.Name,
                Content = blob
            };
        }

        private async Task InstallPackageAsync(string name)
        {
            var content = await _fileContainer.GetAllBytesAsync(name);
            using (var stream = new MemoryStream(content))
            {
                using (ZipArchive archive = new ZipArchive(stream))
                {
                    var nameParts = name.Split('.');
                    name = string.Join(".", nameParts, 0, nameParts.Length - 4); // remove version and extension
                    var pluginFolder = Path.Combine(AppContext.BaseDirectory, $"PlugIns\\{name}");
                    // locate nuspec
                    var nuspec = archive.Entries.FirstOrDefault(e => e.FullName.ToLower().EndsWith(".nuspec"));
                    if (nuspec != null)
                    {
                        if (!Directory.Exists(pluginFolder))
                        {
                            Directory.CreateDirectory(pluginFolder);
                        }

                        // extract nuspec
                        ExtractFile(nuspec, pluginFolder, 0);

                        // extract dlls
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            switch (entry.FullName.Split('/')[0])
                            {
                                case "lib": // lib/net*/...
                                    ExtractFile(entry, pluginFolder, 2);
                                    break;
                            }
                        }
                    }

                }
            }
        }

        private static void ExtractFile(ZipArchiveEntry entry, string folder, int ignoreLeadingSegments)
        {
            string[] segments = entry.FullName.Split('/'); // ZipArchiveEntries always use unix path separator
            string filename = Path.Combine(folder, string.Join(Path.DirectorySeparatorChar, segments, ignoreLeadingSegments, segments.Length - ignoreLeadingSegments));

            // validate path to prevent path traversal
            if (!Path.GetFullPath(filename).StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            entry.ExtractToFile(filename, true);
        }
    }
}
