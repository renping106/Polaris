using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nerd.Abp.PluginManagement.Extensions;
using Serilog;
using Serilog.Events;
using System;
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

            builder.Services.AddPluginManagementService(typeof(BookStoreWebModule), () =>
            {
                var subAppBuilder = WebApplication.CreateBuilder(args);
                subAppBuilder.Host.AddAppSettingsSecretsJson()
                                  .UseDynamicAutofac() 
                                  .UseSerilog();
                return subAppBuilder;
            });

            var app = builder.Build();

            app.RunWithPluginManagement();

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
