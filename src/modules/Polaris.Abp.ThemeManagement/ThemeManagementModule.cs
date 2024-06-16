using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.PluginManagement.Services;
using Polaris.Abp.ThemeManagement.Localization;
using Polaris.Abp.ThemeManagement.Menus;
using Polaris.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup;
using Polaris.Abp.ThemeManagement.Permissions;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Polaris.Abp.PluginManagement;

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
            options.FileSets.AddEmbedded<ThemeManagementModule>("Polaris.Abp.ThemeManagement");
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
            options.Contributors.Add(new PolarisThemeSettingPageContributor());
        });
    }
}
