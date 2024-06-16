using Polaris.Abp.ThemeManagement.Services.Dtos;

namespace Polaris.Abp.ThemeManagement.Services.Interfaces;

public interface IBrandSettingAppService
{
    Task<BrandSettingDto> GetAsync();

    Task UpdateAsync(BrandSettingDto input);
    Task ResetAsync();
}
