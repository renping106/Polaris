namespace Nerd.Abp.PluginManagement.Domain.Interfaces
{
    public interface IWebAppShell
    {
        WebAppShellContext GetContext();
        ValueTask<(bool Success, string Message)> UpdateWebApp(IPlugInDescriptor? plugInToAdd = null);
    }
}
