using Nerd.Abp.ThemeManagement.Domain;
using Nerd.Abp.ThemeManagement.Localization;
using Nerd.Abp.ThemeManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.PluginManagement.Permissions;

public class ThemeManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ThemeManagementPermissions.GroupName, L("Permission:" + ThemeManagementPermissions.GroupName));
        var permission = myGroup.AddPermission(ThemeManagementPermissions.GroupName, L("Permission:" + ThemeManagementPermissions.GroupName), multiTenancySide: MultiTenancySides.Both);
        permission.RequireFeatures(ThemeManagementFeatures.Enable);
        
        var edit = permission.AddChild(ThemeManagementPermissions.Edit, L("Permission:" + ThemeManagementPermissions.Edit), multiTenancySide: MultiTenancySides.Both);
        edit.AddChild(ThemeManagementPermissions.EditBrandSettings, L("Permission:" + ThemeManagementPermissions.EditBrandSettings), multiTenancySide: MultiTenancySides.Both);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ThemeManagementResource>(name);
    }
}
