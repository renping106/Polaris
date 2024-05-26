using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.DynamicPlugin.Services.Interfaces
{
    public interface IFileAppService : IApplicationService
    {
        Task SaveBlobAsync(SaveBlobInputDto input);

        Task<BlobDto> GetBlobAsync(GetBlobRequestDto input);
    }
}