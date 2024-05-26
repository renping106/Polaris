using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.DynamicPlugin.Services.Dtos
{
    public class GetBlobRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
