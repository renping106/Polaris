using Microsoft.Extensions.DependencyInjection;
using Polaris.Abp.NewFireTheme.Bundling;
using Polaris.Abp.NewFireTheme.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Polaris.Abp.NewFireTheme;

[DependsOn(
   typeof(AbpAspNetCoreMvcUiThemeSharedModule),
   typeof(AbpAspNetCoreMvcUiMultiTenancyModule)
)]
public class PolarisNewFireThemeModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(PolarisNewFireThemeModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpThemingOptions>(options =>
        {
            options.Themes.Add<PolarisNewFireTheme>();

            options.DefaultThemeName ??= PolarisNewFireTheme.Name;
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<PolarisNewFireThemeModule>("Polaris.Abp.NewFireTheme");
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new NewFireThemeMainTopToolbarContributor());
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options
                .StyleBundles
                .Add(PolarisNewFireThemeBundles.Styles.Global, bundle =>
                {
                    bundle
                        .AddBaseBundles(StandardBundles.Styles.Global)
                        .AddContributors(typeof(PolarisNewFireThemeGlobalStyleContributor));
                });

            options
                .StyleBundles
                .Add(PolarisNewFireThemeBundles.Styles.Public, bundle =>
                {
                    bundle
                        .AddBaseBundles(StandardBundles.Styles.Global);
                });

            options
                .ScriptBundles
                .Add(PolarisNewFireThemeBundles.Scripts.Global, bundle =>
                {
                    bundle
                        .AddBaseBundles(StandardBundles.Scripts.Global)
                        .AddContributors(typeof(PolarisNewFireThemeGlobalScriptContributor));
                });

        });
    }
}
