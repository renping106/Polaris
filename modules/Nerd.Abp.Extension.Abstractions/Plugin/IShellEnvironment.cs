namespace Nerd.Abp.Extension.Abstractions.Plugin
{
    public interface IShellEnvironment
    {
        IServiceProvider? ShellServiceProvider { get; }
    }
}
