using Volo.Abp.Localization;

namespace Polaris.Abp.NewFireTheme.Themes.NewFire.Components.Toolbar.LanguageSwitch;

public class LanguageSwitchViewComponentModel
{
    public LanguageInfo? CurrentLanguage { get; set; }

    public List<LanguageInfo> OtherLanguages { get; set; } = [];
}
