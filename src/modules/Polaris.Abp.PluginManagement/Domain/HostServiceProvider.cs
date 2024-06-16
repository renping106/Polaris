namespace Polaris.Abp.PluginManagement.Domain;

internal class HostServiceProvider(IServiceProvider instance)
{
    public IServiceProvider Instance { get; } = instance;
}
