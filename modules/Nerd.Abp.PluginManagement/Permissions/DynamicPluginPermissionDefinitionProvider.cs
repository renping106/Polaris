using Nerd.Abp.PluginManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.PluginManagement.Permissions;

public class PluginManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PluginManagementPermissions.GroupName, L("Permission:" + PluginManagementPermissions.GroupName));
        var permission = myGroup.AddPermission(PluginManagementPermissions.GroupName, L("Permission:" + PluginManagementPermissions.GroupName), multiTenancySide: MultiTenancySides.Host);
        permission.AddChild(PluginManagementPermissions.Edit, L("Permission:" + PluginManagementPermissions.Edit), multiTenancySide: MultiTenancySides.Host);
        permission.AddChild(PluginManagementPermissions.Upload, L("Permission:" + PluginManagementPermissions.Upload), multiTenancySide: MultiTenancySides.Host);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PluginManagementResource>(name);
    }
}
