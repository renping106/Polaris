using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Nerd.Abp.DynamicPlugin.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.DynamicPlugin.Pages.DynamicPlugin
{
    public class UploadModel : DynamicPluginPageModel
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

            try
            {
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
            }
            catch (Exception ex)
            {
                Alerts.Danger(
                    text: ex.Message,
                    title: "Failed to install."
                );
                return Page();
            }

            Alerts.Success(
                text: "Uploaded sucessfully."
            );
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
