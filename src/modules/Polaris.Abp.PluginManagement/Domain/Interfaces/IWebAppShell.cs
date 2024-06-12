using Polaris.Abp.Extension.Abstractions.Plugin;

namespace Polaris.Abp.PluginManagement.Domain.Interfaces
{
    public interface IWebAppShell : IShellServiceProvider
    {
        WebAppShellContext GetContext();
        ValueTask<(bool Success, string Message)> UpdateShell();
    }
}
