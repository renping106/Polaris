using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.PluginManagement.Services.Dtos;
using Nerd.Abp.PluginManagement.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.PluginManagement.Pages.PluginManagement
{
    public class UploadModel : PluginManagementPageModel
    {
        [BindProperty]
        public UploadFileDto UploadFileDto { get; set; }

        private readonly IFileAppService _fileAppService;

        public bool Uploaded { get; set; } = false;

        public UploadModel(IFileAppService fileAppService)
        {
            _fileAppService = fileAppService;
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

                    await _fileAppService.SaveBlobAsync(
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
