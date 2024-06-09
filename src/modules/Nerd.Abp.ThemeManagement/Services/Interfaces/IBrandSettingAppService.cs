using Nerd.Abp.ThemeManagement.Services.Dtos;

namespace Nerd.Abp.ThemeManagement.Services.Interfaces
{
    public interface IBrandSettingAppService
    {
        Task<ThemeSettingDto> GetAsync();

        Task UpdateAsync(ThemeSettingDto input);
    }
}
