using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.ThemeManagement.Domain
{
    internal class ThemeSelector : DefaultThemeSelector, IThemeSelector, ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly ILogger<ThemeSelector> _logger;

        public ThemeSelector(IOptions<AbpThemingOptions> options, ISettingManager settingManager, ILogger<ThemeSelector> logger)
            : base(options)
        {
            _settingManager = settingManager;
            _logger = logger;
        }

        public override ThemeInfo GetCurrentThemeInfo()
        {
            var themes = Options.Themes.Values;
            var currentTheme = _settingManager.GetOrNullForCurrentTenantAsync(ThemeManagementSettings.ThemeType, true).GetAwaiter().GetResult();
            var theme = themes.FirstOrDefault(t => t.ThemeType.FullName == currentTheme);
            if (theme == null)
            {
                _logger.LogWarning($"Cannot find the theme {currentTheme}. Use default theme.");
                return base.GetCurrentThemeInfo();
            }
            return theme;
        }
    }
}
