namespace Polaris.Abp.Extension.Abstractions.Plugin;

public interface IShellServiceProvider
{
    IServiceProvider? ServiceProvider { get; }
}
