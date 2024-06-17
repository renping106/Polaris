using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.PluginManagement.Pages;

public class AllowedExtensionsAttribute(string[] extensions) : ValidationAttribute
{
    #region file signature
    private readonly static Dictionary<string, List<byte[]>> _fileSignatures = new()
    {
        { ".zip", new List<byte[]> //also docx, xlsx, pptx, ...
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
            }
        }
    };
    #endregion

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_extensions.Contains(extension.ToLower()))
            {
                return new ValidationResult("This extension is not allowed!");
            }

            using var reader = new BinaryReader(file.OpenReadStream());
            var signatures = _fileSignatures.Values.SelectMany(x => x).ToList();  // flatten all signatures to single list
            var headerBytes = reader.ReadBytes(_fileSignatures.Max(m => m.Value.Max(n => n.Length)));
            var result = signatures.Exists(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
            if (result)
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult("Not supported file.");
    }

    private readonly string[] _extensions = extensions;
}


