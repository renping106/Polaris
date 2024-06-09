using Nerd.Abp.ThemeManagement.Services.Dtos;
using Volo.Abp.Application.Dtos;

namespace Nerd.Abp.ThemeManagement.Services.Interfaces
{
    public interface IThemeAppService
    {
        Task<PagedResultDto<ThemeDto>> GetThemesAsync();
        Task<bool> EnableAsync(string typeName);
    }
}
