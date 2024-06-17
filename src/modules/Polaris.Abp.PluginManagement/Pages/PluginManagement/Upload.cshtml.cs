using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polaris.Abp.PluginManagement.Services.Dtos;
using Polaris.Abp.PluginManagement.Services.Interfaces;

namespace Polaris.Abp.PluginManagement.Pages.PluginManagement;

public class UploadModel(IPackageAppService packageAppService) : PluginManagementPageModel
{
    [BindProperty]
    public UploadFileViewModel UploadFile { get; set; } = new UploadFileViewModel();

    private readonly IPackageAppService _packageAppService = packageAppService;

    public bool Uploaded { get; set; } = false;

    public static void OnGet()
    {
        // Do nothing
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        using (var memoryStream = new MemoryStream())
        {
            if (UploadFile.File != null && UploadFile.File.ContentType!="")
            {
                await UploadFile.File.CopyToAsync(memoryStream);

                await _packageAppService.UploadAsync(
                    new SaveBlobInputDto
                    {
                        Name = UploadFile.Name,
                        Content = memoryStream.ToArray()
                    }
                );
            }
        }
        return Page();
    }
}

public class UploadFileViewModel
{
    [Required]
    [Display(Name = "File")]
    [AllowedExtensions([".nupkg"])]
    public IFormFile? File { get; set; }

    [Required]
    [Display(Name = "Filename")]
    public string Name { get; set; } = string.Empty;
}
