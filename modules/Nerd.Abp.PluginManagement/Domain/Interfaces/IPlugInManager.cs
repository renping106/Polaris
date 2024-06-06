namespace Nerd.Abp.PluginManagement.Domain.Interfaces
{
    public interface IPlugInManager
    {
        IReadOnlyList<IPlugInDescriptor> GetAllPlugIns(bool refresh = false);
        /// <summary>
        ///  Get only enabled plugins without pre-enabled
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IPlugInDescriptor> GetEnabledPlugIns();
        void EnablePlugIn(IPlugInDescriptor plugIn);
        void DisablePlugIn(IPlugInDescriptor plugIn);
        void RemovePlugIn(IPlugInDescriptor plugIn);
        IPlugInDescriptor GetPlugIn(string name);
        void SetPreEnabledPlugIn(IPlugInDescriptor plugIn);
        void ClearPreEnabledPlugIn();
    }
}
