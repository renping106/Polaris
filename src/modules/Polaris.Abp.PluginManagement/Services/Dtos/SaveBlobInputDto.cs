using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.PluginManagement.Services.Dtos;

public class SaveBlobInputDto
{
    public byte[] Content { get; set; } = [];

    [Required]
    public string Name { get; set; } = string.Empty;
}