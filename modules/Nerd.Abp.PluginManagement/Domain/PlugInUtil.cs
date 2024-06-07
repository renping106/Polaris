using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Domain.Models;
using System.Xml.Linq;

namespace Nerd.Abp.PluginManagement.Domain
{
    internal static class PlugInUtil
    {
        private static readonly string folderName = "PlugIns";

        private static XElement GetMetadataElement(XDocument doc)
        {
            var package = doc.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("package", StringComparison.OrdinalIgnoreCase));
            return package?.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("metadata", StringComparison.OrdinalIgnoreCase));
        }

        public static string GetMetaValue(XDocument doc, string name)
        {
            var metadata = GetMetadataElement(doc);

            if (metadata == null)
            {
                throw new InvalidDataException("Invalid nuspec");
            }

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
                    if (plugin.EndsWith("_bak"))
                    {
                        continue;
                    }

                    var nuspecFile = Array.Find(Directory.GetFiles(plugin), t => t.EndsWith(".nuspec"));
                    if (nuspecFile != null)
                    {
                        using StreamReader reader = new(nuspecFile);
                        var nuspec = XDocument.Load(reader);
                        var name = PlugInUtil.GetMetaValue(nuspec, "id");
                        var version = PlugInUtil.GetMetaValue(nuspec, "version");
                        var description = PlugInUtil.GetMetaValue(nuspec, "description");
                        plugInDescriptors.Add(new PlugInDescriptor()
                        {
                            Name = name,
                            Description = description,
                            Version = version,
                            PlugInSource = new FolderSource(plugin)
                        });
                    }
                }
            }

            return plugInDescriptors;
        }
    }
}
