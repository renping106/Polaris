using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Polaris.Abp.Host.Data;

public class HostDbContextFactory : IDesignTimeDbContextFactory<HostDbContext>
{
    public HostDbContext CreateDbContext(string[] args)
    {

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<HostDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new HostDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
