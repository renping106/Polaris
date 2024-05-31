using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Nerd.Abp.DatabaseManagement.Extensions
{
    public static class AbpDbContextOptionsExtension
    {
        public static void ConfigDatabase(this AbpDbContextOptions options, ServiceConfigurationContext context)
        {
            options.Configure(dbContext =>
            {
                var currentDatabase = context.Services.GetRequiredService<ICurrentDatabase>();
                currentDatabase.Provider.UseDatabase(dbContext);
            });
        }
    }
}
