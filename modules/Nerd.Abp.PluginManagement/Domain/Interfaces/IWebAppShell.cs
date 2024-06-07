using Nerd.Abp.Extension.Abstractions.Plugin;

namespace Nerd.Abp.PluginManagement.Domain.Interfaces
{
    public interface IWebAppShell : IShellServiceProvider
    {
        WebAppShellContext GetContext();
        ValueTask<(bool Success, string Message)> UpdateShell();
    }
}
