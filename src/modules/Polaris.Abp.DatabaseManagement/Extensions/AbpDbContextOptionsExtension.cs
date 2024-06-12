using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.DatabaseManagement.Extensions
{
    public static class AbpDbContextOptionsExtension
    {
        public static void ConfigDatabase(this AbpDbContextOptions options)
        {
            options.Configure(context =>
            {
                var currentDatabase = context.ServiceProvider.GetRequiredService<ICurrentDatabase>();
                currentDatabase.Provider.UseDatabase(context);
            });
        }
    }
}
