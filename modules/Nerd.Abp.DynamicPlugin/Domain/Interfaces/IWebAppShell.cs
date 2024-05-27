namespace Nerd.Abp.DynamicPlugin.Domain.Interfaces
{
    public interface IWebAppShell
    {
        WebAppCache GetWebApp();
        ValueTask<(bool Success, string Message)> ResetWebApp();
        ValueTask<(bool Success, string Message)> TestWebApp(IPlugInDescriptor plugInDescriptor);
    }
}
