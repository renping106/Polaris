using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.Extension.Abstractions;
using Polaris.Abp.PluginManagement.Domain;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Polaris.Abp.PluginManagement.Localization;
using Polaris.Abp.PluginManagement.Menus;
using Polaris.Abp.PluginManagement.Permissions;
using Polaris.Abp.PluginManagement.Services;
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

namespace Polaris.Abp.PluginManagement;

[DependsOn(
    typeof(ExtensionAbstractionModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpBlobStoringFileSystemModule)
    )]
public class PluginManagementModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<AbpAspNetCoreMvcOptions>(options =>
        {
            options
                .ConventionalControllers
                .Create(typeof(PluginManagementModule).Assembly);
        });

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(PluginManagementResource), typeof(PluginManagementModule).Assembly);
        });

        ConfigurePlugInViews(context);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new PluginManagementMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PluginManagementModule>("Polaris.Abp.PluginManagement");
        });

        context.Services.AddAutoMapperObjectMapper<PluginManagementModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<PluginManagementModule>(validate: true);
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<PluginManagementResource>("en")
                .AddVirtualJson("/Localization/PluginManagement");
        });

        context.Services.AddAutoMapperObjectMapper<PluginManagementModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<PluginManagementAutoMapperProfile>(validate: true);
        });

        Configure<RazorPagesOptions>(options =>
        {
            //Configure authorization.
            options.Conventions.AuthorizePage("/PluginManagement", PluginManagementPermissions.Default);
            options.Conventions.AuthorizePage("/PluginManagement/Upload", PluginManagementPermissions.Upload);
        });

        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Polaris.Abp.PluginManagement.Pages.PluginManagement.IndexModel>(
                toolbar =>
                {
                    toolbar.AddButton(
                        LocalizableString.Create<PluginManagementResource>("Upload"),
                        icon: "plus",
                        name: "Upload",
                        requiredPolicyName: PluginManagementPermissions.Upload
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

    private void ConfigurePlugInViews(ServiceConfigurationContext context)
    {
        var hostServiceProvider = context.Services.GetSingletonInstanceOrNull<HostServiceProvider>();
        var pluginManager = hostServiceProvider?.Instance.GetRequiredService<IPlugInManager>();

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(PluginManagementModule).Assembly);

            if (pluginManager != null)
            {
                foreach (var plugin in pluginManager.GetEnabledPlugIns())
                {
                    var parts = ((IPlugInContext)plugin.PlugInSource).CompiledRazorAssemblyParts;
                    foreach (var part in parts)
                    {
                        mvcBuilder.PartManager.ApplicationParts.Add(part);
                    }
                }
            }
        });
    }
}
