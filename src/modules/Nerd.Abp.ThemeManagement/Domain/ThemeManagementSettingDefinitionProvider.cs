using Volo.Abp.Settings;

namespace Nerd.Abp.ThemeManagement.Domain
{
    public class ThemeManagementSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            context.Add(new SettingDefinition(ThemeManagementSettings.ThemeType, "Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.BasicTheme"));
        }
    }
}
