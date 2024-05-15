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
                        InitShellHost<TStartupModule>(context, builderInit).GetAwaiter().GetResult();
                    }
                }
            }

            return _shellHost!;
        }

        private static async Task InitShellHost<TStartupModule>(
            HttpContext context,
            Func<WebApplicationBuilder> builderInit)
            where TStartupModule : IAbpModule
        {
            var shellAppBuilder = builderInit();

            var pluginPath = Path.Combine(AppContext.BaseDirectory, "PlugIns");
            await shellAppBuilder.AddApplicationAsync<TStartupModule>(options =>
            {
                if (Path.Exists(pluginPath))
                {
                    foreach (var plugin in Directory.GetDirectories(pluginPath))
                    {
                        options.PlugInSources.AddFolder(plugin);
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

            _shellHost = new ShellHost(shellApp, requestDelegate);
        }
    }
}
