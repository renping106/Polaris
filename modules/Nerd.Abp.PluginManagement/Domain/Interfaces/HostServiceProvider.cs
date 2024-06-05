namespace Nerd.Abp.PluginManagement.Domain.Interfaces
{
    internal class HostServiceProvider
    {
        public IServiceProvider Instance { get; }

        public HostServiceProvider(IServiceProvider provider)
        {
            Instance = provider;
        }
    }
}
