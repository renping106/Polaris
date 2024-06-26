﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.Extension.Abstractions.Plugin;
using Polaris.Abp.PluginManagement.Domain;
using Polaris.Abp.PluginManagement.Domain.Entities;
using Polaris.Abp.PluginManagement.Domain.Interfaces;


namespace Polaris.Abp.PluginManagement.Extensions;

internal class PluginManagementMiddleware(IWebAppShell appShell) : IMiddleware
{
    private readonly IWebAppShell _appShell = appShell;

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
            options.SharedServices.Add(typeof(IShellServiceProvider));
        });

        services.AddTransient<PluginManagementMiddleware>();
        services.AddSingleton<IWebAppShell, WebAppShell>();
        services.AddSingleton<IPlugInManager, PlugInManager>();
        services.AddSingleton<IShellServiceProvider>(x => x.GetRequiredService<IWebAppShell>());

        return services;
    }

    public static IApplicationBuilder RunWithPluginManagement(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<PluginManagementMiddleware>();
    }
}
