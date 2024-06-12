namespace Polaris.Abp.DatabaseManagement.Extensions
{
    public static class GuidExtensions
    {
        public static Guid Normalize(this Guid? guid)
        {
            return guid ?? Guid.Empty;
        }
    }
}
