using System.IO.Compression;
using System.Xml.Linq;
using Polaris.Abp.PluginManagement.Domain.Entities;
using Polaris.Abp.PluginManagement.Domain.Interfaces;

namespace Polaris.Abp.PluginManagement.Domain;

static internal class PlugInPackageUtil
{
    public readonly static string folderName = "PlugIns";
    public readonly static string backupSuffix = "_bak";

    private static XElement? GetMetadataElement(XDocument doc)
    {
        var package = doc.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("package", StringComparison.OrdinalIgnoreCase));
        return package?.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("metadata", StringComparison.OrdinalIgnoreCase));
    }

    public static string GetMetaValue(XDocument doc, string name)
    {
        var metadata = GetMetadataElement(doc) ?? throw new InvalidDataException("Invalid nuspec");
        var node = metadata.Elements().First(e => e.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
        return node.Value;
    }

    public static List<PlugInDescriptor> LoadFromFolder()
    {
        var plugInDescriptors = new List<PlugInDescriptor>();
        var pluginPath = Path.Combine(AppContext.BaseDirectory, folderName);
        if (Path.Exists(pluginPath))
        {
            foreach (var plugin in Directory.GetDirectories(pluginPath))
            {
                // Skip backup if exists
                if (plugin.EndsWith(backupSuffix))
                {
                    continue;
                }

                var nuspecFile = Array.Find(Directory.GetFiles(plugin), t => t.EndsWith(".nuspec"));
                if (nuspecFile != null)
                {
                    using StreamReader reader = new(nuspecFile);
                    plugInDescriptors.Add(ReadDescriptorFromStream(reader));
                }
            }
        }

        return plugInDescriptors;
    }

    public static PlugInDescriptor? LoadPackage(byte[] packageContent)
    {
        PlugInDescriptor? descriptor = null;
        using (var stream = new MemoryStream(packageContent))
        {
            using var archive = new ZipArchive(stream);
            // Locate nuspec
            var nuspecFile = archive.Entries.FirstOrDefault(e => e.FullName.ToLower().EndsWith(".nuspec"));
            if (nuspecFile != null)
            {
                using StreamReader reader = new(nuspecFile.Open());
                descriptor = ReadDescriptorFromStream(reader);
            }
        }
        return descriptor;
    }

    private static PlugInDescriptor ReadDescriptorFromStream(StreamReader reader)
    {
        PlugInDescriptor? descriptor;
        var packageInfo = XDocument.Load(reader);
        var pluginName = GetMetaValue(packageInfo, "id");
        var version = GetMetaValue(packageInfo, "version");
        var description = GetMetaValue(packageInfo, "description");
        var pluginFolder = Path.Combine(AppContext.BaseDirectory, "PlugIns", pluginName);

        descriptor = new PlugInDescriptor()
        {
            Name = pluginName,
            Version = version,
            Description = description,
            PlugInSource = new DynamicPlugInSource(pluginFolder)
        };
        return descriptor;
    }

    public static void RemovePackage(string pluginName)
    {
        var pluginFolder = Path.Combine(AppContext.BaseDirectory, "PlugIns", pluginName);
        Directory.Delete(pluginFolder, true);
    }

    public static void InstallPackage(IPlugInDescriptor plugInDescriptor, byte[] packageContent)
    {
        var pluginFolder = ((DynamicPlugInSource)plugInDescriptor.PlugInSource).Folder;

        using var stream = new MemoryStream(packageContent);
        using var archive = new ZipArchive(stream);
        if (!Directory.Exists(pluginFolder))
        {
            Directory.CreateDirectory(pluginFolder);
            ExtractPackage(archive, pluginFolder);
        }
        else
        {
            BackupFolder(pluginFolder);
            ExtractPackage(archive, pluginFolder);
        }
    }

    public static void RollbackPackage(IPlugInDescriptor plugInDescriptor)
    {
        var pluginFolder = ((DynamicPlugInSource)plugInDescriptor.PlugInSource).Folder;
        var pluginFolderBackup = pluginFolder + backupSuffix;

        // Delete extracted files
        Directory.Delete(pluginFolder, true);
        // Rollback old directory
        Directory.Move(pluginFolderBackup, pluginFolder);
    }

    private static void BackupFolder(string pluginFolder)
    {
        // Backup old directory
        var pluginFolderBackup = pluginFolder + backupSuffix;
        if (Directory.Exists(pluginFolderBackup))
        {
            Directory.Delete(pluginFolderBackup, true);
        }

        Directory.Move(pluginFolder, pluginFolderBackup);
        Directory.CreateDirectory(pluginFolder);
    }

    private static void ExtractPackage(ZipArchive archive, string pluginFolder)
    {
        // Extract info file
        var nuspecFile = archive.Entries.First(e => e.FullName.ToLower().EndsWith(".nuspec"));
        ExtractFile(nuspecFile, pluginFolder, 0);

        // Extract dlls
        foreach (var entry in archive.Entries)
        {
            switch (entry.FullName.Split('/')[0])
            {
                case "lib": // lib/net*/...
                    ExtractFile(entry, pluginFolder, 2);
                    break;
            }
        }
    }

    private static void ExtractFile(ZipArchiveEntry entry, string folder, int ignoreLeadingSegments)
    {
        var segments = entry.FullName.Split('/');
        var filename = Path.Combine(folder, string.Join(Path.DirectorySeparatorChar, segments, ignoreLeadingSegments, segments.Length - ignoreLeadingSegments));

        // Validate path to prevent path traversal
        if (!Path.GetFullPath(filename).StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        entry.ExtractToFile(filename, true);
    }
}
