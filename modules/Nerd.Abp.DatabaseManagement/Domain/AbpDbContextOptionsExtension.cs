using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Domain
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
