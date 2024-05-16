using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DynamicPlugin.Shell;
using Volo.Abp.Modularity;

namespace Nerd.Abp.DynamicPlugin.Extensions
{
    public static class DynamicPluginEndpointExtension
    {
        public static async Task UseDynamicPlugins<TStartupModule>(
            this IApplicationBuilder app,
            HttpContext context,
            Func<WebApplicationBuilder> builderInit)
            where TStartupModule : IAbpModule
        {
            var webAppShell = WebAppShell.GetShell<TStartupModule>(builderInit);

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
}
