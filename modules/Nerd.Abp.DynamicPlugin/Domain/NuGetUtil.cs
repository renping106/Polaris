using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nerd.Abp.DynamicPlugin.Domain
{
    internal static class NuGetUtil
    {
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
    }
}
