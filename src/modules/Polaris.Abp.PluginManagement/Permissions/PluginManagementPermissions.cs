using Volo.Abp.Reflection;

namespace Polaris.Abp.PluginManagement.Permissions;

public static class PluginManagementPermissions
{
    public const string Default = "PluginManagement";
    public const string Edit = Default + ".Edit";
    public const string Upload = Default + ".Upload";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(PluginManagementPermissions));
    }
}
