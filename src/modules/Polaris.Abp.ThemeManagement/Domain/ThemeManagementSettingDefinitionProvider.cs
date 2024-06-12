using Volo.Abp.Settings;

namespace Polaris.Abp.ThemeManagement.Domain
{
    public class ThemeManagementSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            context.Add(new SettingDefinition(ThemeManagementSettings.ThemeType, "Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.BasicTheme"));
            context.Add(new SettingDefinition(ThemeManagementSettings.LogoUrl, "/images/logo/logo-dark-thumbnail.png"));
            context.Add(new SettingDefinition(ThemeManagementSettings.LogoReverseUrl, "/images/logo/logo-light-thumbnail.png"));
        }
    }
}
