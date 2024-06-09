using Nerd.Abp.ThemeManagement.Services.Dtos;

namespace Nerd.Abp.ThemeManagement.Services.Interfaces
{
    public interface IBrandSettingAppService
    {
        Task<BrandSettingDto> GetAsync();

        Task UpdateAsync(BrandSettingDto input);
        Task ResetAsync();
    }
}
