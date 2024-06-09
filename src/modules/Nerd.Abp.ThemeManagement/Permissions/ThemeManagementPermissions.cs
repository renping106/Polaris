using Volo.Abp.Reflection;

namespace Nerd.Abp.ThemeManagement.Permissions
{
    public static class ThemeManagementPermissions
    {
        public const string GroupName = "ThemeManagement";
        public const string Edit = GroupName + ".Edit";
        public const string EditBrandSettings = GroupName + ".EditBrandSettings";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(ThemeManagementPermissions));
        }
    }
}
