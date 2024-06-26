﻿using Volo.Abp.Settings;

namespace Polaris.Abp.ThemeManagement.Domain;

public class DatabaseManagementSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        context.Add(new SettingDefinition(DatabaseManagementSettings.DatabaseProvider));
        context.Add(new SettingDefinition(DatabaseManagementSettings.SiteName, "Polaris"));
        context.Add(new SettingDefinition(DatabaseManagementSettings.DatabaseVersion, "0"));

        context.Add(new SettingDefinition(DatabaseManagementSettings.DefaultAdminEmail));
        context.Add(new SettingDefinition(DatabaseManagementSettings.DefaultAdminPassword, isEncrypted: true));
    }
}
