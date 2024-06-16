using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace Polaris.Abp.ThemeManagement.Domain;

internal class ThemeSelector : DefaultThemeSelector, IThemeSelector, ITransientDependency
{
    private readonly ILogger<ThemeSelector> _logger;
    private readonly ISettingProvider _settingProvider;

    public ThemeSelector(IOptions<AbpThemingOptions> options, ILogger<ThemeSelector> logger, ISettingProvider settingProvider)
        : base(options)
    {
        _logger = logger;
        _settingProvider = settingProvider;
    }

    public override ThemeInfo GetCurrentThemeInfo()
    {
        var themes = Options.Themes.Values;
        var currentTheme = _settingProvider.GetOrNullAsync(ThemeManagementSettings.ThemeType).GetAwaiter().GetResult();
        var theme = themes.FirstOrDefault(t => t.ThemeType.FullName == currentTheme);
        if (theme == null)
        {
            _logger.LogWarning($"Cannot find the theme {currentTheme}. Use default theme.");
            return base.GetCurrentThemeInfo();
        }
        return theme;
    }
}
