using Volo.Abp.Application.Dtos;

namespace Nerd.Abp.PluginManagement.Services.Dtos
{
    public class PlugInDescriptorDto : EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string Version { get; set; } = string.Empty;
    }
}
