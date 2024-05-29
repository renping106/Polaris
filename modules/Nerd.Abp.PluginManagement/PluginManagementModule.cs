﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.PluginManagement.Localization;
using Nerd.Abp.PluginManagement.Menus;
using Nerd.Abp.PluginManagement.Permissions;
using Nerd.Abp.PluginManagement.Services;
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

namespace Nerd.Abp.PluginManagement
{
    [DependsOn(
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

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                //Add plugin assembly
                mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(PluginManagementModule).Assembly));

                //Add CompiledRazorAssemblyPart if the PlugIn module contains razor views.
                mvcBuilder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(typeof(PluginManagementModule).Assembly));
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new PluginManagementMenuContributor());
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<PluginManagementModule>("Nerd.Abp.PluginManagement");
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
                options.Conventions.AuthorizePage("/PluginManagement", PluginManagementPermissions.GroupName);
                options.Conventions.AuthorizePage("/PluginManagement/Upload", PluginManagementPermissions.Upload);
            });

            Configure<AbpPageToolbarOptions>(options =>
            {
                options.Configure<Nerd.Abp.PluginManagement.Pages.PluginManagement.IndexModel>(
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
    }
}