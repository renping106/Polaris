using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace Polaris.Abp.DatabaseManagement.Pages.Setup
{
    public class SetupViewModel
    {
        [Required]
        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = string.Empty;

        [BindProperty]
        public string DatabaseProvider { get; set; } = string.Empty;

        [Required]
        [BindProperty]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; } = string.Empty;

        [BindNever]
        [Display(Name = "User Name")]
        public string UserName { get; } = "admin";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Password Confirmation")]
        public string PasswordConfirmation { get; set; } = string.Empty;

        [Display(Name = "Time Zone")]
        public string Timezone { get; set; } = "UTC";

        public List<SelectListItem> TimeZoneItems { get; set; } = new List<SelectListItem>();

        [Display(Name = "User Host Setting")]
        public bool UseHostSetting { get; set; } = true;
    }
}
