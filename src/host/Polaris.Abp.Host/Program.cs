using Polaris.Abp.PluginManagement.Extensions;
using Serilog;
using Serilog.Events;

namespace Polaris.Abp.Host;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console());

        Log.Logger = loggerConfiguration.CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddPluginManagementService(typeof(PolarisAbpHostModule), () =>
            {
                var subAppBuilder = WebApplication.CreateBuilder(args);
                subAppBuilder.Host.AddAppSettingsSecretsJson()
                                  .UseDynamicAutofac()
                                  .UseSerilog();
                return subAppBuilder;
            });

            var app = builder.Build();

            app.RunWithPluginManagement();

            Log.Information("Starting Polaris.Abp.Host.");
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Polaris.Abp.Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
