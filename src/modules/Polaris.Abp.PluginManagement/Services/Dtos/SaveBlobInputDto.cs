using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.PluginManagement.Services.Dtos;

public class SaveBlobInputDto
{
    public byte[] Content { get; set; } = new byte[0];

    [Required]
    public string Name { get; set; } = string.Empty;
}