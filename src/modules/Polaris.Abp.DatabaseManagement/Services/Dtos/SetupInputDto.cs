using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.DatabaseManagement.Services.Dtos;

public class SetupInputDto
{
    [Required]
    public required string SiteName { get; set; }
    [Required]
    public required string ConnectionString { get; set; }
    [Required]
    public required string DatabaseProvider { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Email { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public bool UseHostSetting { get; set; }
}
