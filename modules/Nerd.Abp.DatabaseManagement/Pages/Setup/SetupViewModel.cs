using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;


namespace Nerd.Abp.DatabaseManagement.Pages.Setup
{
    public class SetupViewModel
    {
        [Required]
        [Display(Name = "Site Name")]
        public string SiteName { get; set; }

        public string Description { get; set; }

        [BindProperty]
        public string DatabaseProvider { get; set; }

        [Required]
        [BindProperty]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// True if the database configuration is preset and can't be changed or displayed on the Setup screen.
        /// </summary>
        [BindNever]
        public bool DatabaseConfigurationPreset { get; set; }

        [BindNever]
        [Display(Name = "User Name")]
        public string UserName { get; } = "admin";

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Password Confirmation")]
        public string PasswordConfirmation { get; set; }

        public string SiteTimeZone { get; set; }
    }
}
