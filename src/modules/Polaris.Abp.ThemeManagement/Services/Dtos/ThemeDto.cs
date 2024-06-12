namespace Polaris.Abp.ThemeManagement.Services.Dtos
{
    public class ThemeDto
    {
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }
}
