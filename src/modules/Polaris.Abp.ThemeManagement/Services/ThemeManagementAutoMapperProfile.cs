using Polaris.Abp.ThemeManagement.Services.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using static Polaris.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup.PolarisThemeSettingViewComponent;

namespace Polaris.Abp.PluginManagement.Services;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BrandSettingDtoToThemeSettingViewModelMapper : MapperBase<BrandSettingDto, ThemeSettingViewModel>
{
    public override partial ThemeSettingViewModel Map(BrandSettingDto source);

    public override partial void Map(BrandSettingDto source, ThemeSettingViewModel destination);
}