namespace Nerd.Abp.DynamicPlugin.Domain.Plugin
{
    public interface IPlugInManager
    {
        IReadOnlyList<IPlugInDescriptor> GetAllPlugIns(bool refresh = false);
        IReadOnlyList<IPlugInDescriptor> GetEnabledPlugIns();
        void EnablePlugIn(IPlugInDescriptor plugIn);
        void DisablePlugIn(IPlugInDescriptor plugIn);
    }
}
