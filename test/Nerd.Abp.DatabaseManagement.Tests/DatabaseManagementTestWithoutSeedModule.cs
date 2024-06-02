using Nerd.Abp.DatabaseManagement.Extensions;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Tests
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(DatabaseManagementModule)
    )]
    public class DatabaseManagementTestWithoutSeedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbContextOptions>(options =>
            {
                options.ConfigDatabase();
            });
        }
    }
}