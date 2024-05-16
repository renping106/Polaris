using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.DynamicPlugin.Shell
{
    internal static class WebAppShell
    {
        private static ShellHost? _shellHost;
        private static readonly object instanceLock = new object();
        public static List<string> Plugins { get; private set; } = new List<string>();

        public static ShellHost GetShell<TStartupModule>(Func<WebApplicationBuilder> builderInit)
            where TStartupModule : IAbpModule
        {
            if (_shellHost == null)
            {
                lock (instanceLock)
                {
                    if (_shellHost == null)
                    {
                        _shellHost = InitShellHostAsync(typeof(TStartupModule), builderInit).GetAwaiter().GetResult();
                    }
                }
            }

            return _shellHost!;
        }

        public static async ValueTask<(bool Success, string Message)> UpdateShellHostAsync()
        {
            try
            {
                var builderInit = _shellHost!.BuilderInit;
                var newShell = await InitShellHostAsync(_shellHost!.StartupModuleType, builderInit, true);
                if (newShell != null)
                {
                    _shellHost = newShell;
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        private static async ValueTask<ShellHost> InitShellHostAsync(
            Type startupModuleType,
            Func<WebApplicationBuilder> builderInit,
            bool loadPlugin = false)
        {
            var shellAppBuilder = builderInit();

            var pluginPath = Path.Combine(AppContext.BaseDirectory, "PlugIns");
            await shellAppBuilder.AddApplicationAsync(startupModuleType, options =>
            {
                if (loadPlugin && Path.Exists(pluginPath))
                {
                    foreach (var plugin in Directory.GetDirectories(pluginPath))
                    {
                        if (!Plugins.Contains(plugin))
                        {
                            options.PlugInSources.AddFolder(plugin);
                            Plugins.Add(plugin);
                        }
                    }
                }
            });

            var shellApp = shellAppBuilder.Build();

            Action<IApplicationBuilder> configure = async (builder) =>
            {
                await shellApp.InitializeApplicationAsync();
            };

            configure(shellApp);

            // Build the request pipeline.
            var requestDelegate = ((IApplicationBuilder)shellApp).Build();

            return new ShellHost(shellApp.Services, requestDelegate, builderInit, startupModuleType);
        }
    }
}
