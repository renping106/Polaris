namespace Nerd.Abp.DynamicPlugin.Domain.Interfaces
{
    public interface IPlugInManager
    {
        IReadOnlyList<IPlugInDescriptor> GetAllPlugIns(bool refresh = false);
        IReadOnlyList<IPlugInDescriptor> GetEnabledPlugIns();
        void EnablePlugIn(IPlugInDescriptor plugIn);
        void DisablePlugIn(IPlugInDescriptor plugIn);
        void RemovePlugIn(IPlugInDescriptor plugIn);
    }
}
