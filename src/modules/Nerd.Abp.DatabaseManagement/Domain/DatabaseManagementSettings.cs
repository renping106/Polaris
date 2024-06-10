namespace Nerd.Abp.ThemeManagement.Domain
{
    public static class DatabaseManagementSettings
    {
        private const string Prefix = "Nerd.Abp.DatabaseManagement";

        //Add your own setting names here. Example:
        public const string DatabaseProvider = Prefix + ".DatabaseProvider";
        public const string SiteName = Prefix + ".SiteName";
        public const string DatabaseVersion = Prefix + ".DatabaseVersion";

        public const string DefaultAdminEmail = Prefix + ".DefaultAdminEmail";
        public const string DefaultAdminPassword = Prefix + ".DefaultAdminPassword";
    }
}
