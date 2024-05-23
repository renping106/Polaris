using Autofac.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DynamicPlugin.Domain.Interfaces;

namespace Nerd.Abp.DynamicPlugin.Domain
{
    internal sealed class WebAppShell
    {
        private WebAppCache? _webAppCache;
        private readonly object instanceLock = new object();
        private static readonly Lazy<WebAppShell> instance = new(() => new WebAppShell());

        public static WebAppShell Instance => instance.Value;

        public WebAppCache GetShell(Type startupModuleTyp, Func<WebApplicationBuilder> builderInit)
        {
            if (_webAppCache == null)
            {
                lock (instanceLock)
                {
                    if (_webAppCache == null)
                    {
                        _webAppCache = InitShellAsync(startupModuleTyp, builderInit).GetAwaiter().GetResult();
                    }
                }
            }

            return _webAppCache!;
        }

        public async ValueTask<(bool Success, string Message)> UpdateShellAsync()
        {
            try
            {
                var builderInit = _webAppCache!.BuilderInit;
                var startupModuleTyp = _webAppCache!.StartupModuleTyp;
                var newShell = await InitShellAsync(startupModuleTyp, builderInit);
                if (newShell != null)
                {
                    _webAppCache = newShell;
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        public async ValueTask<(bool Success, string Message)> TestShellAsync(IPlugInDescriptor plugInDescriptor)
        {
            try
            {
                var builderInit = _webAppCache!.BuilderInit;
                var startupModuleTyp = _webAppCache!.StartupModuleTyp;
                _ = await InitShellAsync(startupModuleTyp, builderInit, plugInDescriptor);                
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        private static async ValueTask<WebAppCache> InitShellAsync(
            Type startupModuleTyp,
            Func<WebApplicationBuilder> builderInit,
            IPlugInDescriptor? externalPlugin = null)
        {
            var shellAppBuilder = builderInit();
            shellAppBuilder.Services.AddSingleton<IPlugInManager, PlugInManager>();

            await shellAppBuilder.AddApplicationAsync(startupModuleTyp, options =>
            {
                var serviceProvider = shellAppBuilder.Services.BuildServiceProvider();
                var plugInManager = serviceProvider.GetRequiredService<IPlugInManager>();

                var enabledPlugIns = plugInManager.GetEnabledPlugIns();
                foreach (var enabledPlug in enabledPlugIns)
                {
                    options.PlugInSources.Add(enabledPlug.PlugInSource);
                }

                if (externalPlugin != null)
                {
                    options.PlugInSources.Add(externalPlugin.PlugInSource);
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

            return new WebAppCache(shellApp.Services, requestDelegate, startupModuleTyp, builderInit);
        }
    }
}
