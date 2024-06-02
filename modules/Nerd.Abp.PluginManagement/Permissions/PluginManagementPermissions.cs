﻿using Volo.Abp.Reflection;

namespace Nerd.Abp.PluginManagement.Permissions;

public class PluginManagementPermissions
{
    public const string Default = "PluginManagement";
    public const string Edit = Default + ".Edit";
    public const string Upload = Default + ".Upload";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(PluginManagementPermissions));
    }
}
