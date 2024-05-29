using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Ping.Nerd.Web.Filters;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Nerd.Abp.DatabaseManagement
{
    public class DatabaseManagementModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
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
        }
    }
}
