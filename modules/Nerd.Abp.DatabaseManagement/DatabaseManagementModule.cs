﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nerd.Abp.DatabaseManagement.Data;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Sqlite;
using Nerd.Abp.DatabaseManagement.SqlServer;
using Ping.Nerd.Web.Filters;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AutoMapper;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.VirtualFileSystem;

namespace Nerd.Abp.DatabaseManagement
{
    [DependsOn(
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
                //Add plugin assembly
                mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(DatabaseManagementModule).Assembly));

                //Add CompiledRazorAssemblyPart
                mvcBuilder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(typeof(DatabaseManagementModule).Assembly));
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Filters.AddService(typeof(SetupAsyncPageFilter));
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<DatabaseManagementModule>("Nerd.Abp.DatabaseManagement");
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

            // make AbpUnitOfWorkDefaultOptions as scoped
            context.Services.AddScoped(provider =>
            {
                var currentProvider = provider.GetRequiredService<ICurrentDatabase>().Provider;
                var tranOption = Options.Create(new AbpUnitOfWorkDefaultOptions());
                tranOption.Value.TransactionBehavior = currentProvider.UnitOfWorkTransactionBehaviorOption;
                return tranOption;
            });

            // make AbpSequentialGuidGeneratorOptions as scoped
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
        }
    }
}
