using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DynamicPlugin.Domain;
using Nerd.Abp.DynamicPlugin.Domain.Interfaces;

namespace Nerd.Abp.DynamicPlugin.Extensions
{
    internal class DynamicPluginMiddleware : IMiddleware
    {
        private readonly IWebAppShell _appShell;

        public DynamicPluginMiddleware(IWebAppShell appShell)
        {
            _appShell = appShell;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var webAppShell = _appShell.GetWebApp();

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

    public static class DynamicPluginMiddlewareExtensions
    {
        public static IServiceCollection AddDynamicPluginService(this IServiceCollection services, 
            Type startupModuleType, 
            Func<WebApplicationBuilder> builderInit)
        {
            services.Configure<WebShellOptions>(options =>
            {
                options.StartupModuleTyp = startupModuleType;
                options.BuilderInit = builderInit;
                options.SharedServices.Add(typeof(IWebAppShell));
                options.SharedServices.Add(typeof(IPlugInManager));
            });

            services.AddTransient<DynamicPluginMiddleware>();
            services.AddSingleton<IWebAppShell, WebAppShell>();
            services.AddSingleton<IPlugInManager, PlugInManager>();

            return services;
        }

        public static IApplicationBuilder RunWithDynamicPlugin(
            this IApplicationBuilder app)
            => app.UseMiddleware<DynamicPluginMiddleware>();
    }
}
