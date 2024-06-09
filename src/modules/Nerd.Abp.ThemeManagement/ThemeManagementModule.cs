﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.PluginManagement.Services;
using Nerd.Abp.ThemeManagement.Localization;
using Nerd.Abp.ThemeManagement.Menus;
using Nerd.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup;
using Nerd.Abp.ThemeManagement.Permissions;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Nerd.Abp.PluginManagement
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcUiThemeSharedModule),
        typeof(AbpAutoMapperModule)
        )]
    public class ThemeManagementModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpAspNetCoreMvcOptions>(options =>
            {
                options
                    .ConventionalControllers
                    .Create(typeof(ThemeManagementModule).Assembly);
            });

            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(ThemeManagementResource), typeof(ThemeManagementModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(ThemeManagementModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new ThemeManagementMenuContributor());
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<ThemeManagementModule>("Nerd.Abp.ThemeManagement");
            });

            context.Services.AddAutoMapperObjectMapper<ThemeManagementModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<ThemeManagementModule>(validate: true);
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<ThemeManagementResource>("en")
                    .AddVirtualJson("/Localization/ThemeManagement");
            });

            context.Services.AddAutoMapperObjectMapper<ThemeManagementModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<ThemeManagementAutoMapperProfile>(validate: true);
            });

            Configure<RazorPagesOptions>(options =>
            {
                //Configure authorization.
                options.Conventions.AuthorizePage("/ThemeManagement", ThemeManagementPermissions.GroupName);
            });

            Configure<SettingManagementPageOptions>(options =>
            {
                options.Contributors.Add(new NerdThemeSettingPageContributor());
            });
        }
    }
}
