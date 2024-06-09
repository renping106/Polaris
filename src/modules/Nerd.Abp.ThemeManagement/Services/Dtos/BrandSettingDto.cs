using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.ThemeManagement.Services.Dtos
{
    public class BrandSettingDto
    {
        [MaxLength(50)]
        public string? SiteName { get; set; }

        [MaxLength(200)]
        public string? LogoUrl { get; set; }

        [MaxLength(200)]
        public string? LogoReverseUrl { get; set; }
    }
}
