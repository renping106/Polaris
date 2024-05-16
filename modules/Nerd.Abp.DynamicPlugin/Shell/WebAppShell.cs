using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
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

        public static ShellHost GetShell<TStartupModule>(
            HttpContext context,
            Func<WebApplicationBuilder> builderInit)
            where TStartupModule : IAbpModule
        {
            if (_shellHost == null)
            {
                lock (instanceLock)
                {
                    if (_shellHost == null)
                    {
                        _shellHost = InitShellHost(typeof(TStartupModule), context, builderInit).GetAwaiter().GetResult();
                    }
                }
            }

            return _shellHost!;
        }

        public static async Task UpdateShellHost(HttpContext context)
        {
            var builderInit = _shellHost!.BuilderInit;
            var newShell = await InitShellHost(_shellHost!.StartupModuleType, context, builderInit);
            if (newShell != null)
            {
                _shellHost = newShell;
            }
        }

        private static async Task<ShellHost> InitShellHost(
            Type startupModuleType,
            HttpContext context,
            Func<WebApplicationBuilder> builderInit)
        {
            var shellAppBuilder = builderInit();

            var pluginPath = Path.Combine(AppContext.BaseDirectory, "PlugIns");
            await shellAppBuilder.AddApplicationAsync(startupModuleType, options =>
            {
                if (Path.Exists(pluginPath))
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
            var applicationBuilderFactory = shellApp.Services.GetRequiredService<IApplicationBuilderFactory>();
            var moduleAppBuilder = applicationBuilderFactory.CreateBuilder(context.Features);

            Action<IApplicationBuilder> configure = async (builder) =>
            {
                await moduleAppBuilder.InitializeApplicationAsync();
            };

            configure(moduleAppBuilder);

            // Build the request pipeline.
            var requestDelegate = moduleAppBuilder.Build();

            return new ShellHost(shellApp.Services, requestDelegate, builderInit, startupModuleType);
        }
    }
}
