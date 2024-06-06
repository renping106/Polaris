
using Nerd.Abp.Extension.Abstractions.Plugin;

namespace Nerd.Abp.PluginManagement.Domain
{
    internal class ShellEnvironment : IShellEnvironment
    {
        public IServiceProvider? HostServiceProvider { get; set; }

        public IServiceProvider? ShellServiceProvider { get; set; }
    }
}
