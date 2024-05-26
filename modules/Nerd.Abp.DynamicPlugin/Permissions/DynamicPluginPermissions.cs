using Volo.Abp.Reflection;

namespace Nerd.Abp.DynamicPlugin.Permissions;

public class DynamicPluginPermissions
{
    public const string GroupName = "DynamicPlugin";
    public const string Edit = GroupName + ".Edit";
    public const string Upload = GroupName + ".Upload";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(DynamicPluginPermissions));
    }
}
