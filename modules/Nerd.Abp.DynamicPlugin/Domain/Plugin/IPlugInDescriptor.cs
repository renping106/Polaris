using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.DynamicPlugin.Domain.Plugin
{
    public interface IPlugInDescriptor
    {
        string Name { get; set; }
        string Description { get; set; }
        bool IsEnabled { get; set; }
        IPlugInSource PlugInSource { get; set; }
    }
}
