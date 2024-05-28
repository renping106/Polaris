using Microsoft.Extensions.Options;
using Nerd.Abp.ThemeManagement.Services.Dtos;
using Nerd.Abp.ThemeManagement.Services.Interfaces;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;

namespace Nerd.Abp.ThemeManagement.Services
{
    public class ThemeAppService : ThemeManagementAppServiceBase, IThemeAppServoce
    {
        private readonly IThemeManager _themeManager;
        private readonly AbpThemingOptions _options;

        public ThemeAppService(IOptions<AbpThemingOptions> options, IThemeManager themeManager)
        {
            _options = options.Value;
            _themeManager = themeManager;
        }

        public async Task<PagedResultDto<ThemeDto>> GetThemes()
        {
            var themes = _options.Themes.ToList();
            return new PagedResultDto<ThemeDto>(
                   themes.Count,
                   themes.Select(t => new ThemeDto() { Name = t.Value.Name, TypeName = t.Key.FullName, IsEnabled = _options.DefaultThemeName == t.Value.Name }).ToList()
                );
        }

        public async Task<bool> Enable(string typeName)
        {
            var themes = _options.Themes.ToList();
            var target = themes.Find(t => t.Key.FullName == typeName);
            _options.DefaultThemeName = target.Value.Name;
            return true;
        }
    }
}
