namespace Nerd.Abp.Extension.Abstractions.Plugin
{
    public interface IShellEnvironment
    {
        IServiceProvider? HostServiceProvider { get; }
        IServiceProvider? ShellServiceProvider { get; }
    }
}
