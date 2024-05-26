using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerd.Abp.DynamicPlugin.Extensions;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Nerd.BookStore.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.Run(async context =>
            {
                await app.UseDynamicPlugins(context, typeof(BookStoreWebModule), () =>
                {
                    var subAppBuilder = WebApplication.CreateBuilder(args);
                    subAppBuilder.Host.AddAppSettingsSecretsJson()
                                   .UseMyAutofac()
                                   .UseSerilog();
                    return subAppBuilder;
                });
            });

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

public static class AbpAutofacHostBuilderExtensions
{
    public static IHostBuilder UseMyAutofac(this IHostBuilder hostBuilder)
    {
        var context = AssemblyLoadContext.All.FirstOrDefault(t => t.GetType().Name == "TestAssemblyLoadContext");
        if (context != null)
        {
            context.Unload();
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Autofac.dll");
        var assemblyLoadContext = new TestAssemblyLoadContext(path);
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
        //var factory = Activator.CreateInstance(AbpAutofacServiceProviderFactory, containerBuilder);
        var factory = methods[0].Invoke([containerBuilder]);
        //var newFac = (IServiceProviderFactory<Object>)factory;
        //var providerType = typeof(IServiceProviderFactory<>);
        //var genericFactory = providerType.MakeGenericType(ContainerBuilder);
        //var ins = Activator.CreateInstance(genericFactory) as IServiceProviderFactory<Object>;

        var aaa = hostBuilder.ConfigureServices((_, services) =>
        {
            services.AddObjectAccessor(containerBuilder);
        });

        var methodInfos = typeof(IHostBuilder).GetMethods();//"UseServiceProviderFactory"
        var methodInfo = methodInfos.FirstOrDefault(t => t.GetParameters().Count() == 1 && t.Name == "UseServiceProviderFactory");

        methodInfo = methodInfo.MakeGenericMethod(ContainerBuilder);

        return (IHostBuilder)methodInfo.Invoke(aaa, [factory]);

        //return hostBuilder.ConfigureServices((_, services) =>
        //{
        //    services.AddObjectAccessor(containerBuilder);
        //})
        //    .UseServiceProviderFactory(newFac);
    }
}

class TestAssemblyLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;

    public TestAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
    }

    protected override Assembly? Load(AssemblyName name)
    {
        //string? assemblyPath = _resolver.ResolveAssemblyToPath(name);
        //if (assemblyPath != null)
        //{
        //    return LoadFromAssemblyPath(assemblyPath);
        //}

        return null;
    }
}
