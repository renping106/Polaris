namespace Polaris.Abp.PluginManagement.Domain
{
    internal class HostServiceProvider
    {
        public IServiceProvider Instance { get; }

        public HostServiceProvider(IServiceProvider instance)
        {
            Instance = instance;
        }
    }
}
