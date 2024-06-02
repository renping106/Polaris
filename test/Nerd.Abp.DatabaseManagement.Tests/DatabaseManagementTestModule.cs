using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Threading;
using Nerd.Abp.DatabaseManagement.Extensions;

namespace Nerd.Abp.DatabaseManagement.Tests
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(DatabaseManagementModule)
    )]
    public class DatabaseManagementTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbContextOptions>(options =>
            {
                options.ConfigDatabase();
            });

            context.Services.AddAlwaysAllowAuthorization();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            SeedTestData(context);
        }

        private static void SeedTestData(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(async () =>
            {
                var setupAppService = context.ServiceProvider.GetRequiredService<ISetupAppService>();
                var setupInput = new SetupInputDto()
                {
                    SiteName = "Test",
                    ConnectionString = "InMemory",
                    DatabaseProvider = "InMemory",
                    Password = "Test",
                    Email = "test@test.com"
                };
                await setupAppService.InstallAsync(setupInput, null);
            });
        }
    }
}