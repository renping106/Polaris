using Volo.Abp.Application.Dtos;

namespace Nerd.Abp.DynamicPlugin.Services.Dtos
{
    public class GetPluginsInputDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
