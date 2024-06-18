using System.Text.Json.Serialization;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Volo.Abp.Modularity.PlugIns;

namespace Polaris.Abp.PluginManagement.Domain.Entities;

internal class PlugInDescriptor : IPlugInDescriptor
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string Version { get; set; } = string.Empty;
    public string? AbpVersion { get; set; }

    [JsonIgnore]
    public IPlugInSource PlugInSource { get; set; } = new DynamicPlugInSource("");

    public IPlugInDescriptor Clone()
    {
        var folderSource = new DynamicPlugInSource(((DynamicPlugInSource)PlugInSource).Folder);
        return new PlugInDescriptor()
        {
            Name = Name,
            Version = Version,
            PlugInSource = folderSource,
            AbpVersion = AbpVersion
        };
    }
}
