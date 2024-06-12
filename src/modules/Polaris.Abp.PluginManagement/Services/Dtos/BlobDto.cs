namespace Polaris.Abp.PluginManagement.Services.Dtos
{
    public class BlobDto
    {
        public byte[] Content { get; set; } = new byte[0];

        public string Name { get; set; } = string.Empty;
    }
}
