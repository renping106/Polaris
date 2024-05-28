using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.PluginManagement.Domain;

namespace Nerd.Abp.PluginManagement.Extensions
{
    //public static class PluginManagementEndpointExtension
    //{
    //    public static async ValueTask UsePluginManagements(
    //        this IApplicationBuilder app,
    //        HttpContext context,
    //        Type startupModuleType,
    //        Func<WebApplicationBuilder> builderInit)
    //    {
    //        var webAppShell = WebAppShell.Instance.GetShell(startupModuleType, builderInit);

    //        // Workaround to fix asp-page tag helpers in plugin
    //        var scope = webAppShell.Services.CreateScope();
    //        context.RequestServices = scope.ServiceProvider;

    //        // Workaround to fix IFeatureCollection is disposed error
    //        var httpContextFactory = scope.ServiceProvider.GetRequiredService<IHttpContextFactory>();
    //        var httpContext = httpContextFactory.Create(context.Features);

    //        // Workacound to fix null httpcontext
    //        var httpAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    //        httpAccessor.HttpContext = httpContext;

    //        // Run real pipelines
    //        await webAppShell.RequestDelegate(context);
    //    }
    //}
}
