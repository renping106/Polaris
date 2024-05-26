using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.DynamicPlugin.Domain.Interfaces
{
    public interface IPlugInDescriptor
    {
        string Name { get; set; }
        string Description { get; set; }
        bool IsEnabled { get; set; }
        string Version { get; set; }
        string AbpVersion { get; set; }
        IPlugInSource PlugInSource { get; set; }
    }
}
