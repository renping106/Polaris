using AutoMapper;
using Nerd.Abp.ThemeManagement.Services.Dtos;
using static Nerd.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup.NerdThemeSettingViewComponent;

namespace Nerd.Abp.PluginManagement.Services
{
    public class ThemeManagementAutoMapperProfile : Profile
    {
        public ThemeManagementAutoMapperProfile()
        {
            CreateMap<ThemeSettingDto, ThemeSettingViewModel>();
        }
    }
}
