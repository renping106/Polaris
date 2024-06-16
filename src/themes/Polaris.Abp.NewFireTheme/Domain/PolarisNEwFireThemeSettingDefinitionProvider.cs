using Volo.Abp.Settings;

namespace Polaris.Abp.NewFireTheme.Domain;

public class PolarisNEwFireThemeSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        context.Add(new SettingDefinition(PolarisNewFireThemeSettings.SiteName, "Polaris"));
        context.Add(new SettingDefinition(PolarisNewFireThemeSettings.LogoUrl, null));
        context.Add(new SettingDefinition(PolarisNewFireThemeSettings.LogoReverseUrl, null));
    }
}
