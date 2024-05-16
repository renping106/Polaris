using Nerd.Abp.DynamicPlugin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Nerd.Abp.DynamicPlugin.Permissions;

public class DynamicPluginPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DynamicPluginPermissions.GroupName, L("Permission:DynamicPlugin"));
        myGroup.AddPermission(DynamicPluginPermissions.List);
        myGroup.AddPermission(DynamicPluginPermissions.Edit);
        myGroup.AddPermission(DynamicPluginPermissions.Install);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DynamicPluginResource>(name);
    }
}
