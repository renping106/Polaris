using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polaris.Abp.PluginManagement.Services.Dtos;
using Polaris.Abp.PluginManagement.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.PluginManagement.Pages.PluginManagement
{
    public class UploadModel : PluginManagementPageModel
    {
        [BindProperty]
        public UploadFileDto UploadFileDto { get; set; }

        private readonly IPackageAppService _packageAppService;

        public bool Uploaded { get; set; } = false;

        public UploadModel(IPackageAppService packageAppService)
        {
            _packageAppService = packageAppService;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            ValidateModel();

            using (var memoryStream = new MemoryStream())
            {
                if (UploadFileDto.File != null)
                {
                    await UploadFileDto.File.CopyToAsync(memoryStream);

                    await _packageAppService.UploadAsync(
                        new SaveBlobInputDto
                        {
                            Name = UploadFileDto.Name,
                            Content = memoryStream.ToArray()
                        }
                    );
                }
            }
            return Page();
        }
    }

    public class UploadFileDto
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile? File { get; set; }

        [Required]
        [Display(Name = "Filename")]
        public string Name { get; set; } = string.Empty;
    }
}
