﻿using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polaris.Abp.PluginManagement.Domain;

namespace Polaris.Abp.PluginManagement.Extensions;

public static class AutoFacHostBuilderExtension
{
    /// <summary>
    /// We need to load Volo.Abp.Autofac and Castle.Core into a different AssemblyLoadContext.
    /// This will allow us unload generated proxies
    /// Make sure to remove DependsOn to them.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder UseDynamicAutoFac(this IHostBuilder hostBuilder)
    {
        try
        {
            var context = AssemblyLoadContext.All.FirstOrDefault(t => t.GetType().Name == nameof(AutoFacLoadContext));
            if (context != null)
            {
                var contextReference = new WeakReference(context, trackResurrection: true);

                context.Unload();

                for (var i = 0; contextReference.IsAlive && (i < 10); i++)
                {
#pragma warning disable S1215
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
#pragma warning restore S1215
                }
            }

            // Dynamic load them to make the generated proxies are in a collectable AssemblyLoadContext
            var autofacContext = new AutoFacLoadContext();

            var assembly = LoadAssembly(autofacContext, "Autofac.dll");
            // Find ContainerBuilder
            var ContainerBuilder = Array.Find(assembly.GetTypes(), t => t.Name == "ContainerBuilder");
            var containerBuilder = Activator.CreateInstance(ContainerBuilder!);

            LoadAssembly(autofacContext, "Autofac.Extensions.DependencyInjection.dll");
            LoadAssembly(autofacContext, "Autofac.Extras.DynamicProxy.dll");
            LoadAssembly(autofacContext, "Volo.Abp.Castle.Core.dll");
            LoadAssembly(autofacContext, "Castle.Core.dll");
            LoadAssembly(autofacContext, "Castle.Core.AsyncInterceptor.dll");

            assembly = LoadAssembly(autofacContext, "Volo.Abp.Autofac.dll");
            // Find AbpAutofacServiceProviderFactory
            var AbpAutofacServiceProviderFactory = Array.Find(assembly.GetTypes(), t => t.Name == "AbpAutofacServiceProviderFactory");
            var methods = AbpAutofacServiceProviderFactory!.GetConstructors();
            var factory = methods[0].Invoke([containerBuilder]);

            var host = hostBuilder.ConfigureServices((_, services) =>
            {
                services.AddObjectAccessor(containerBuilder);
            });

            // Dynamically call IHostBuilder.UseServiceProviderFactory
            var methodInfos = typeof(IHostBuilder).GetMethods();
            var methodInfo = Array.Find(methodInfos, t => t.GetParameters().Length == 1 && t.Name == "UseServiceProviderFactory");

            methodInfo = methodInfo?.MakeGenericMethod(ContainerBuilder!);

            return methodInfo?.Invoke(host, [factory]) as IHostBuilder ?? hostBuilder;
        }
        catch (Exception)
        {
            // Don't throw
        }
        return hostBuilder;
    }

    private static Assembly LoadAssembly(AssemblyLoadContext context, string dllName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, dllName);
        return context.LoadFromAssemblyPath(path);
    }
}
