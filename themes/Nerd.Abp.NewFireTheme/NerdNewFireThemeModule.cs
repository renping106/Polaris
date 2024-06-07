using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.NewFireTheme.Bundling;
using Nerd.Abp.NewFireTheme.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Nerd.Abp.NewFireTheme
{
    [DependsOn(
       typeof(AbpAspNetCoreMvcUiThemeSharedModule),
       typeof(AbpAspNetCoreMvcUiMultiTenancyModule)
    )]
    public class NerdNewFireThemeModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(NerdNewFireThemeModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpThemingOptions>(options =>
            {
                options.Themes.Add<NerdNewFireTheme>();

                options.DefaultThemeName ??= NerdNewFireTheme.Name;
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<NerdNewFireThemeModule>("Nerd.Abp.NewFireTheme");
            });

            Configure<AbpToolbarOptions>(options =>
            {
                options.Contributors.Add(new NewFireThemeMainTopToolbarContributor());
            });

            Configure<AbpBundlingOptions>(options =>
            {
                options
                    .StyleBundles
                    .Add(NerdNewFireThemeBundles.Styles.Global, bundle =>
                    {
                        bundle
                            .AddBaseBundles(StandardBundles.Styles.Global)
                            .AddContributors(typeof(NerdNewFireThemeGlobalStyleContributor));
                    });

                options
                    .StyleBundles
                    .Add(NerdNewFireThemeBundles.Styles.Public, bundle =>
                    {
                        bundle
                            .AddBaseBundles(StandardBundles.Styles.Global);
                    });

                options
                    .ScriptBundles
                    .Add(NerdNewFireThemeBundles.Scripts.Global, bundle =>
                    {
                        bundle
                            .AddBaseBundles(StandardBundles.Scripts.Global)
                            .AddContributors(typeof(NerdNewFireThemeGlobalScriptContributor));
                    });

            });
        }
    }
}
