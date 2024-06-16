namespace Polaris.Abp.PluginManagement.Domain.Interfaces;

public interface IPlugInManager
{
    Task DisablePlugInAsync(string plugInName);
    Task<(bool Success, string Message)> EnablePlugInAsync(string plugInName);
    IReadOnlyList<IPlugInDescriptor> GetAllPlugIns(bool refresh = false);
    /// <summary>
    ///  Get only enabled plugins without pre-enabled
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<IPlugInDescriptor> GetEnabledPlugIns();
    IPlugInDescriptor GetPlugIn(string name);
    Task RefreshPlugInAsync(IPlugInDescriptor installedPlugIn);
    void RemovePlugIn(string plugInName);
}
