using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nerd.Abp.DynamicPlugin.Extensions;
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
            var app = builder.Build();

            app.Run(async context =>
            {
                await app.UseDynamicPlugins<BookStoreWebModule>(context, () =>
                {
                    var appBuilder = WebApplication.CreateBuilder(args);
                    appBuilder.Host.AddAppSettingsSecretsJson()
                                   .UseAutofac()
                                   .UseSerilog();
                    return appBuilder;
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
