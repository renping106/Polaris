using System.Collections.Generic;
using Volo.Abp.Localization;

namespace Nerd.Abp.NewFireTheme.Themes.NewFire.Components.Toolbar.LanguageSwitch;

public class LanguageSwitchViewComponentModel
{
    public LanguageInfo CurrentLanguage { get; set; }

    public List<LanguageInfo> OtherLanguages { get; set; }
}
