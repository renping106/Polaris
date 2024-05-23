using Nerd.Abp.DynamicPlugin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.DynamicPlugin.Permissions;

public class DynamicPluginPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DynamicPluginPermissions.GroupName, L("Permission:" + DynamicPluginPermissions.GroupName));
        var permission = myGroup.AddPermission(DynamicPluginPermissions.GroupName, L("Permission:" + DynamicPluginPermissions.GroupName), multiTenancySide: MultiTenancySides.Host);
        permission.AddChild(DynamicPluginPermissions.Edit, L("Permission:" + DynamicPluginPermissions.Edit), multiTenancySide: MultiTenancySides.Host);
        permission.AddChild(DynamicPluginPermissions.Upload, L("Permission:" + DynamicPluginPermissions.Upload), multiTenancySide: MultiTenancySides.Host);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DynamicPluginResource>(name);
    }
}
