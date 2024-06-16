using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace Polaris.Abp.ThemeManagement.Domain;

internal class ThemeSelector(IOptions<AbpThemingOptions> options, ILogger<ThemeSelector> logger, ISettingProvider settingProvider) 
    : DefaultThemeSelector(options), IThemeSelector, ITransientDependency
{
    private readonly ILogger<ThemeSelector> _logger = logger;
    private readonly ISettingProvider _settingProvider = settingProvider;

    public override ThemeInfo GetCurrentThemeInfo()
    {
        var themes = Options.Themes.Values;
        var currentTheme = _settingProvider.GetOrNullAsync(ThemeManagementSettings.ThemeType).GetAwaiter().GetResult();
        var theme = themes.FirstOrDefault(t => t.ThemeType.FullName == currentTheme);
        if (theme == null)
        {
            _logger.LogWarning("Cannot find the theme {CurrentTheme}. Use default theme.", currentTheme);
            return base.GetCurrentThemeInfo();
        }
        return theme;
    }
}
