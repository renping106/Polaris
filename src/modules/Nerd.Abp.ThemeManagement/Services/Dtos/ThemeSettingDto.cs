using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.ThemeManagement.Services.Dtos
{
    public class ThemeSettingDto
    {
        [MaxLength(200)]
        public string? LogoUrl { get; set; }

        [MaxLength(200)]
        public string? LogoReverseUrl { get; set; }
    }
}
