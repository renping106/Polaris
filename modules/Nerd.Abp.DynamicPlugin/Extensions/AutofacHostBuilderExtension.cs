using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerd.Abp.DynamicPlugin.Domain;
using System.Runtime.Loader;

namespace Nerd.Abp.DynamicPlugin.Extensions
{
    public static class AutofacHostBuilderExtension
    {
        public static IHostBuilder UseDynamicAutofac(this IHostBuilder hostBuilder)
        {
            var context = AssemblyLoadContext.All.FirstOrDefault(t => t.GetType().Name == "AutofacLoadContext");
            if (context != null)
            {
                context.Unload();
            }

            // Dynamic load them to make the generated proxies are in a collectable AssemblyLoadContext
            var path = Path.Combine(AppContext.BaseDirectory, "Autofac.dll");
            var assemblyLoadContext = new AutofacLoadContext();
            var assembly = assemblyLoadContext.LoadFromAssemblyPath(path);

            var ContainerBuilder = assembly.GetTypes().FirstOrDefault(t => t.Name == "ContainerBuilder");
            var containerBuilder = Activator.CreateInstance(ContainerBuilder);

            path = Path.Combine(AppContext.BaseDirectory, "Autofac.Extensions.DependencyInjection.dll");
            assembly = assemblyLoadContext.LoadFromAssemblyPath(path);

            path = Path.Combine(AppContext.BaseDirectory, "Autofac.Extras.DynamicProxy.dll");
            assembly = assemblyLoadContext.LoadFromAssemblyPath(path);

            path = Path.Combine(AppContext.BaseDirectory, "Volo.Abp.Castle.Core.dll");
            assembly = assemblyLoadContext.LoadFromAssemblyPath(path);

            path = Path.Combine(AppContext.BaseDirectory, "Castle.Core.dll");
            assembly = assemblyLoadContext.LoadFromAssemblyPath(path);

            path = Path.Combine(AppContext.BaseDirectory, "Castle.Core.AsyncInterceptor.dll");
            assembly = assemblyLoadContext.LoadFromAssemblyPath(path);

            path = Path.Combine(AppContext.BaseDirectory, "Volo.Abp.Autofac.dll");
            assembly = assemblyLoadContext.LoadFromAssemblyPath(path);
            var AbpAutofacServiceProviderFactory = assembly.GetTypes().FirstOrDefault(t => t.Name == "AbpAutofacServiceProviderFactory");

            var methods = AbpAutofacServiceProviderFactory.GetConstructors();
            var factory = methods[0].Invoke([containerBuilder]);

            var host = hostBuilder.ConfigureServices((_, services) =>
            {
                services.AddObjectAccessor(containerBuilder);
            });

            var methodInfos = typeof(IHostBuilder).GetMethods(); //"UseServiceProviderFactory"
            var methodInfo = methodInfos.FirstOrDefault(t => t.GetParameters().Count() == 1 && t.Name == "UseServiceProviderFactory");

            methodInfo = methodInfo.MakeGenericMethod(ContainerBuilder);

            return (IHostBuilder)methodInfo.Invoke(host, [factory]);
        }
    }
}
