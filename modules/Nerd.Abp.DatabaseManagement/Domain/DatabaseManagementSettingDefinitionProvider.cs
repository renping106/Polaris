using Volo.Abp.Settings;

namespace Nerd.Abp.ThemeManagement.Domain
{
    public class DatabaseManagementSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            context.Add(new SettingDefinition(DatabaseManagementSettings.DatabaseProvider));
            context.Add(new SettingDefinition(DatabaseManagementSettings.SiteName, "Nerd"));
        }
    }
}
