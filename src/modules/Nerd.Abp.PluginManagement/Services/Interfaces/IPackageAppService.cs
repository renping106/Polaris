using Nerd.Abp.PluginManagement.Services.Dtos;
using Volo.Abp.Application.Services;

namespace Nerd.Abp.PluginManagement.Services.Interfaces
{
    public interface IPackageAppService : IApplicationService
    {
        Task UploadAsync(SaveBlobInputDto input);

        Task<BlobDto> GetAsync(GetBlobRequestDto input);
        void RemovePlugIn(string pluginName);
    }
}