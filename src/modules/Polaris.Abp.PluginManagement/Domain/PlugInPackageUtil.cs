using Microsoft.CodeAnalysis;
using Polaris.Abp.PluginManagement.Domain.Entities;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Polaris.Abp.PluginManagement.Domain;

static internal class PlugInPackageUtil
{
    private readonly static string _backupSuffix = "_bak";
    private readonly static string _listFileName = "installed-lib.txt";
    private readonly static string _plugInFolderName = "PlugIns";

    public static string GetMetaValue(XDocument doc, string name)
    {
        var metadata = GetMetadataElement(doc) ?? throw new InvalidDataException("Invalid nuspec");
        var node = metadata.Elements().First(e => e.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
        return node.Value;
    }

    public static Version? GetPackageAbpVersion(XDocument doc)
    {
        var dependencies = GetDependenciesElement(doc);
        if (dependencies != null)
        {
            foreach (var group in dependencies.Elements())
            {
                foreach (var dependency in group.Elements())
                {
                    var package = dependency.Attributes().FirstOrDefault(t => t.Name.LocalName == "id");
                    if (package?.Value == "Volo.Abp.Core")
                    {
                        var version = dependency.Attributes().FirstOrDefault(t => t.Name.LocalName == "version");
                        return version != null ? Version.Parse(version.Value) : null;
                    }
                }
            }
        }

        return null;
    }

    public static Version? GetCurrentAbpVersion()
    {
        var assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(t => t.ManifestModule.Name == "Volo.Abp.Core.dll");
        var version = assembly?.GetName().Version;
        return version;
    }

    public static void InstallPackage(IPlugInDescriptor plugInDescriptor, byte[] packageContent, string webRootPath)
    {
        var pluginFolder = ((DynamicPlugInSource)plugInDescriptor.PlugInSource).Folder;

        using var stream = new MemoryStream(packageContent);
        using var archive = new ZipArchive(stream);
        if (!Directory.Exists(pluginFolder))
        {
            Directory.CreateDirectory(pluginFolder);
            ExtractPackage(archive, pluginFolder, webRootPath);
        }
        else
        {
            BackupFolder(pluginFolder);
            ExtractPackage(archive, pluginFolder, webRootPath);
        }
    }

    public static List<PlugInDescriptor> LoadFromFolder()
    {
        var plugInDescriptors = new List<PlugInDescriptor>();
        var pluginPath = Path.Combine(AppContext.BaseDirectory, _plugInFolderName);
        if (Path.Exists(pluginPath))
        {
            foreach (var plugin in Directory.GetDirectories(pluginPath))
            {
                // Skip backup if exists
                if (plugin.EndsWith(_backupSuffix))
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

    public static void RemovePackage(string pluginName)
    {
        var pluginFolder = Path.Combine(AppContext.BaseDirectory, _plugInFolderName, pluginName);
        var listFilePath = Path.Combine(pluginFolder, _listFileName);

        if (File.Exists(listFilePath))
        {
            var installedLibs = File.ReadLines(listFilePath);
            foreach (var installedLib in installedLibs)
            {
                var folder = "wwwroot\\libs\\";
                var pathParts = installedLib.Split(folder);
                var dirName = installedLib.Substring(0, installedLib.IndexOf(folder) + folder.Length - 1);
                var subDirName = pathParts[1].Substring(0, pathParts[1].IndexOf("\\"));
                var libDir = Path.Combine(dirName, subDirName);
                if (Directory.Exists(libDir))
                {
                    Directory.Delete(libDir, true);
                }
            }
        }

        Directory.Delete(pluginFolder, true);
    }

    public static void RollbackPackage(IPlugInDescriptor plugInDescriptor)
    {
        var pluginFolder = ((DynamicPlugInSource)plugInDescriptor.PlugInSource).Folder;
        var pluginFolderBackup = pluginFolder + _backupSuffix;

        // Delete extracted files
        Directory.Delete(pluginFolder, true);
        // Rollback old directory
        Directory.Move(pluginFolderBackup, pluginFolder);
    }

    private static void BackupFolder(string pluginFolder)
    {
        // Backup old directory
        var pluginFolderBackup = pluginFolder + _backupSuffix;
        if (Directory.Exists(pluginFolderBackup))
        {
            Directory.Delete(pluginFolderBackup, true);
        }

        Directory.Move(pluginFolder, pluginFolderBackup);
        Directory.CreateDirectory(pluginFolder);
    }

    private static string? ExtractFile(ZipArchiveEntry entry, string baseFolder, int ignoreLeadingSegments, bool createFolder = false)
    {
        var segments = entry.FullName.Split('/');
        var filename = Path.Combine(baseFolder, string.Join(Path.DirectorySeparatorChar, segments, ignoreLeadingSegments, segments.Length - ignoreLeadingSegments));

        // Validate path to prevent path traversal
        if (!Path.GetFullPath(filename).StartsWith(baseFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (createFolder)
        {
            var folderPath = Path.GetDirectoryName(filename);
            if (folderPath != null && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        if (File.Exists(filename))
        {
            return null;
        }

        entry.ExtractToFile(filename, true);

        return filename;
    }

    private static void ExtractPackage(ZipArchive archive, string pluginFolder, string webRootPath)
    {
        // Extract info file
        var nuspecFile = archive.Entries.First(e => e.FullName.ToLower().EndsWith(".nuspec"));
        ExtractFile(nuspecFile, pluginFolder, 0);

        var files = new List<string>();
        // Extract dlls and client libs
        foreach (var entry in archive.Entries)
        {
            switch (entry.FullName.Split('/')[0])
            {
                case "lib": // lib/net*/...
                    ExtractFile(entry, pluginFolder, 2);
                    break;
                case "wwwroot": // wwwroot/...
                    var name = ExtractFile(entry, webRootPath, 1, true);
                    if (name != null)
                    {
                        files.Add(name);
                    }
                    break;
            }
        }

        SaveFileList(pluginFolder, files);
    }

    private static XElement? GetMetadataElement(XDocument doc)
    {
        var package = doc.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("package", StringComparison.OrdinalIgnoreCase));
        return package?.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("metadata", StringComparison.OrdinalIgnoreCase));
    }

    private static XElement? GetDependenciesElement(XDocument doc)
    {
        var metadata = GetMetadataElement(doc);
        return metadata?.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("dependencies", StringComparison.OrdinalIgnoreCase));
    }

    private static PlugInDescriptor ReadDescriptorFromStream(StreamReader reader)
    {
        PlugInDescriptor? descriptor;
        var packageInfo = XDocument.Load(reader);
        var pluginName = GetMetaValue(packageInfo, "id");
        var version = GetMetaValue(packageInfo, "version");
        var description = GetMetaValue(packageInfo, "description");
        var pluginFolder = Path.Combine(AppContext.BaseDirectory, _plugInFolderName, pluginName);
        var abpVersion = GetPackageAbpVersion(packageInfo);

        descriptor = new PlugInDescriptor()
        {
            Name = pluginName,
            Version = version,
            Description = description,
            PlugInSource = new DynamicPlugInSource(pluginFolder),
            AbpVersion = abpVersion?.ToString(),
        };
        return descriptor;
    }

    private static void SaveFileList(string pluginFolder, List<string> files)
    {
        if (files.Count > 0)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in files)
            {
                stringBuilder.AppendLine(item);
            }
            var listFilePath = Path.Combine(pluginFolder, _listFileName);
            File.WriteAllText(listFilePath, stringBuilder.ToString(), Encoding.UTF8);
        }
    }
}
