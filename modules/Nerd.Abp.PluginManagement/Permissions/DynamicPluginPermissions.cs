using Volo.Abp.Reflection;

namespace Nerd.Abp.PluginManagement.Permissions;

public class PluginManagementPermissions
{
    public const string GroupName = "PluginManagement";
    public const string Edit = GroupName + ".Edit";
    public const string Upload = GroupName + ".Upload";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(PluginManagementPermissions));
    }
}
