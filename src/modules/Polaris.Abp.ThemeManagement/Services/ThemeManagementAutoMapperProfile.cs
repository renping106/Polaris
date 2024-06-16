using AutoMapper;
using Polaris.Abp.ThemeManagement.Services.Dtos;
using static Polaris.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup.PolarisThemeSettingViewComponent;

namespace Polaris.Abp.PluginManagement.Services;

public class ThemeManagementAutoMapperProfile : Profile
{
    public ThemeManagementAutoMapperProfile()
    {
        CreateMap<BrandSettingDto, ThemeSettingViewModel>();
    }
}
