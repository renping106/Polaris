using Polaris.Abp.PluginManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Polaris.Abp.PluginManagement.Permissions;

public class PluginManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PluginManagementPermissions.Default, L("Permission:" + PluginManagementPermissions.Default));
        var permission = myGroup.AddPermission(PluginManagementPermissions.Default, L("Permission:" + PluginManagementPermissions.Default), multiTenancySide: MultiTenancySides.Host);
        permission.AddChild(PluginManagementPermissions.Edit, L("Permission:" + PluginManagementPermissions.Edit), multiTenancySide: MultiTenancySides.Host);
        permission.AddChild(PluginManagementPermissions.Upload, L("Permission:" + PluginManagementPermissions.Upload), multiTenancySide: MultiTenancySides.Host);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PluginManagementResource>(name);
    }
}
