using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.Extension.Abstractions.Plugin;
using Nerd.Abp.PluginManagement.Domain;
using Nerd.Abp.PluginManagement.Domain.Interfaces;


namespace Nerd.Abp.PluginManagement.Extensions
{
    internal class PluginManagementMiddleware : IMiddleware
    {
        private readonly IWebAppShell _appShell;
        private readonly IServiceProvider _serviceProvider;

        public PluginManagementMiddleware(IWebAppShell appShell, IServiceProvider serviceProvider)
        {
            _appShell = appShell;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var webAppShell = _appShell.GetContext();

            // Workaround to fix asp-page tag helpers in plugin
            var scope = webAppShell.Services.CreateScope();
            context.RequestServices = scope.ServiceProvider;

            // Workaround to fix IFeatureCollection is disposed error
            var httpContextFactory = scope.ServiceProvider.GetRequiredService<IHttpContextFactory>();
            var httpContext = httpContextFactory.Create(context.Features);

            // Workacound to fix null httpcontext
            var httpAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            httpAccessor.HttpContext = httpContext;

            // Run real pipelines
            await webAppShell.RequestDelegate(context);
        }
    }

    public static class PluginManagementMiddlewareExtensions
    {
        public static IServiceCollection AddPluginManagementService(this IServiceCollection services, 
            Type startupModuleType, 
            Func<WebApplicationBuilder> initBuiler)
        {
            services.Configure<WebShellOptions>(options =>
            {
                options.StartupModuleTyp = startupModuleType;
                options.InitBuilder = initBuiler;
                options.SharedServices.Add(typeof(IWebAppShell));
                options.SharedServices.Add(typeof(IPlugInManager));
                options.SharedServices.Add(typeof(IShellEnvironment));
            });

            services.AddTransient<PluginManagementMiddleware>();
            services.AddSingleton<IWebAppShell, WebAppShell>();
            services.AddSingleton<IPlugInManager, PlugInManager>();
            services.AddSingleton<IShellEnvironment>(new ShellEnvironment());

            return services;
        }

        public static IApplicationBuilder RunWithPluginManagement(
            this IApplicationBuilder app)
            => app.UseMiddleware<PluginManagementMiddleware>();
    }
}
