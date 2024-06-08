using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Domain.Entities;
using System.Runtime.Loader;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.PluginManagement.Domain
{
    internal class WebAppShell : IWebAppShell
    {
        private WebAppShellContext? _context;
        private readonly IServiceProvider _hostServiceProvider;
        private readonly WebShellOptions _options;
        private readonly object instanceLock = new object();

        public WebAppShell(IOptions<WebShellOptions> webShellOptions, IServiceProvider hostServiceProvider)
        {
            _options = webShellOptions.Value;
            _hostServiceProvider = hostServiceProvider;
        }

        public IServiceProvider? ServiceProvider => _context?.Services;

        public WebAppShellContext GetContext()
        {
            if (_context == null)
            {
                lock (instanceLock)
                {
                    if (_context == null)
                    {
                        _context = InitShellAsync().GetAwaiter().GetResult();
                    }
                }
            }
            return _context!;
        }

        public async ValueTask<(bool Success, string Message)> UpdateShell()
        {
            try
            {
                var newShell = await InitShellAsync();
                if (newShell != null)
                {
                    _context = newShell;
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        private async ValueTask<WebAppShellContext> InitShellAsync()
        {
            var shellAppBuilder = _options.InitBuilder();

            RegisterSharedServices(shellAppBuilder);

            await shellAppBuilder.AddApplicationAsync(_options.StartupModuleTyp, options =>
            {
                // Core modules for dynamic
                var context = AssemblyLoadContext.All.FirstOrDefault(t => t.GetType().Name == nameof(AutofacLoadContext));
                if (context != null)
                {
                    foreach (var item in context.Assemblies)
                    {
                        var moduleTypes = item.GetTypes().Where(t => t.IsAssignableTo(typeof(AbpModule)));
                        if (moduleTypes.Any())
                        {
                            options.PlugInSources.AddTypes(moduleTypes.ToArray());
                        }
                    }
                }

                // Enabled plugins
                var plugInManager = _hostServiceProvider.GetRequiredService<IPlugInManager>();
                var enabledPlugIns = plugInManager.GetEnabledPlugIns();
                foreach (var enabledPlug in enabledPlugIns)
                {
                    options.PlugInSources.Add(enabledPlug.PlugInSource);
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

            return new WebAppShellContext(shellApp.Services, requestDelegate);
        }

        private void RegisterSharedServices(WebApplicationBuilder shellAppBuilder)
        {
            foreach (var item in _options.SharedServices)
            {
                shellAppBuilder.Services.AddSingleton(item, provider =>
                {
                    return _hostServiceProvider.GetRequiredService(item);
                });
            }

            shellAppBuilder.Services.AddSingleton(new HostServiceProvider(_hostServiceProvider));
        }
    }
}
