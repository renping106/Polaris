using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ping.Polaris.Web.Filters;
using Polaris.Abp.DatabaseManagement.Data;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.DatabaseManagement.Sqlite;
using Polaris.Abp.DatabaseManagement.SqlServer;
using Polaris.Abp.Extension.Abstractions;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AutoMapper;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.VirtualFileSystem;

namespace Polaris.Abp.DatabaseManagement;

[DependsOn(
typeof(ExtensionAbstractionModule),
typeof(AbpTenantManagementApplicationModule),
typeof(DatabaseManagementSqlServerModule),
typeof(DatabaseManagementSqliteModule)
)]
public class DatabaseManagementModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // Register all dbcontexts
        context.Services.AddConventionalRegistrar(new EfCoreDbConventionalRegistrar());

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(DatabaseManagementModule), typeof(DatabaseManagementModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(DatabaseManagementModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<MvcOptions>(mvcOptions =>
        {
            mvcOptions.Filters.AddService(typeof(TenantStateAsyncPageFilter));
            mvcOptions.Filters.AddService(typeof(TenantUpdateAsyncPageFilter));
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<DatabaseManagementModule>("Polaris.Abp.DatabaseManagement");
        });

        context.Services.AddAutoMapperObjectMapper<DatabaseManagementModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DatabaseManagementModule>(validate: true);
        });

        context.Services.AddAbpDbContext<DatabaseManagementDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        // Make AbpUnitOfWorkDefaultOptions as scoped
        context.Services.AddScoped(provider =>
        {
            var currentProvider = provider.GetRequiredService<ICurrentDatabase>().Provider;
            var tranOption = Options.Create(new AbpUnitOfWorkDefaultOptions());
            tranOption.Value.TransactionBehavior = currentProvider.UnitOfWorkTransactionBehaviorOption;
            return tranOption;
        });

        // Make AbpSequentialGuidGeneratorOptions as scoped
        context.Services.AddScoped(provider =>
        {
            var currentProvider = provider.GetRequiredService<ICurrentDatabase>().Provider;
            var guidGeneratorOption = Options.Create(new AbpSequentialGuidGeneratorOptions());
            guidGeneratorOption.Value.DefaultSequentialGuidType = currentProvider.SequentialGuidTypeOption;
            return guidGeneratorOption;
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles.Configure(
                "Volo.Abp.TenantManagement.Web.Pages.TenantManagement.Tenants.IndexModel",
                bundle =>
                {
                    bundle.AddFiles("/Pages/TenantManagement/Tenants/index-extension.js");
                });
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });
    }
}
