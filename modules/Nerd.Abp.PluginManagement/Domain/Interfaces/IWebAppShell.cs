namespace Nerd.Abp.PluginManagement.Domain.Interfaces
{
    public interface IWebAppShell
    {
        WebAppShellContext GetContext();
        ValueTask<(bool Success, string Message)> ResetWebApp();
        ValueTask<(bool Success, string Message)> TestWebApp(IPlugInDescriptor plugInDescriptor);
    }
}
