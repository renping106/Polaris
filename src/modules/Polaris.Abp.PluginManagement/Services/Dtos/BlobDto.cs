namespace Polaris.Abp.PluginManagement.Services.Dtos;

public class BlobDto
{
    public byte[] Content { get; set; } = [];

    public string Name { get; set; } = string.Empty;
}
