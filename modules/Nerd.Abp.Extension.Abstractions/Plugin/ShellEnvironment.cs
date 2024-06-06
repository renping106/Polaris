namespace Nerd.Abp.Extension.Abstractions.Plugin
{
    public class ShellEnvironment
    {
        public IServiceProvider HostServiceProvider { get; }
        public IServiceProvider? ShellServiceProvider { get; set; }

        public ShellEnvironment(IServiceProvider provider)
        {
            HostServiceProvider = provider;
        }
    }
}
