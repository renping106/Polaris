using Volo.Abp.Settings;

namespace Nerd.Abp.NewFireTheme.Domain
{
    public class NerdNEwFireThemeSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            context.Add(new SettingDefinition(NerdNewFireThemeSettings.SiteName, "Nerd"));
            context.Add(new SettingDefinition(NerdNewFireThemeSettings.LogoUrl, null));
            context.Add(new SettingDefinition(NerdNewFireThemeSettings.LogoReverseUrl, null));
        }
    }
}
