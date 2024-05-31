using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.DatabaseManagement.Services.Dtos
{
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
    }
}
