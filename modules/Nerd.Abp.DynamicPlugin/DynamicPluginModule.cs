using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DynamicPlugin.Localization;
using Nerd.Abp.DynamicPlugin.Menus;
using Nerd.Abp.DynamicPlugin.Permissions;
using Nerd.Abp.DynamicPlugin.Services;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Nerd.Abp.DynamicPlugin
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcUiThemeSharedModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpBlobStoringFileSystemModule)
        )]
    public class DynamicPluginModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpAspNetCoreMvcOptions>(options =>
            {
                options
                    .ConventionalControllers
                    .Create(typeof(DynamicPluginModule).Assembly);
            });

            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(DynamicPluginResource), typeof(DynamicPluginModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                //Add plugin assembly
                mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(DynamicPluginModule).Assembly));

                //Add CompiledRazorAssemblyPart if the PlugIn module contains razor views.
                mvcBuilder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(typeof(DynamicPluginModule).Assembly));
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new DynamicPluginMenuContributor());
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<DynamicPluginModule>("Nerd.Abp.DynamicPlugin");
            });

            context.Services.AddAutoMapperObjectMapper<DynamicPluginModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<DynamicPluginModule>(validate: true);
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<DynamicPluginResource>("en")
                    .AddVirtualJson("/Localization/DynamicPlugin");
            });

            context.Services.AddAutoMapperObjectMapper<DynamicPluginModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<DynamicPluginAutoMapperProfile>(validate: true);
            });

            Configure<RazorPagesOptions>(options =>
            {
                //Configure authorization.
                options.Conventions.AuthorizePage("/DynamicPlugIn", DynamicPluginPermissions.GroupName);
                options.Conventions.AuthorizePage("/DynamicPlugIn/Upload", DynamicPluginPermissions.Upload);
            });

            Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin.IndexModel>(
                    toolbar =>
                    {
                        toolbar.AddButton(
                            LocalizableString.Create<DynamicPluginResource>("Upload"),
                            icon: "plus",
                            name: "Upload",
                            requiredPolicyName: DynamicPluginPermissions.Upload
                        );
                    }
                );
            });

            var packageFolder = Path.Combine(AppContext.BaseDirectory, "Packages");
            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    container.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = packageFolder;
                    });
                });
            });
        }
    }
}
