using Nerd.Abp.PluginManagement.Services.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.PluginManagement.Services.Interfaces
{
    public interface IFileAppService : IApplicationService
    {
        Task SaveBlobAsync(SaveBlobInputDto input);

        Task<BlobDto> GetBlobAsync(GetBlobRequestDto input);
    }
}