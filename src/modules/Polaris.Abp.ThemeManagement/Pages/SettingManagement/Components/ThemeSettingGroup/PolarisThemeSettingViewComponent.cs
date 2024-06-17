using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Polaris.Abp.ThemeManagement.Services.Dtos;
using Polaris.Abp.ThemeManagement.Services.Interfaces;
using Volo.Abp.AspNetCore.Mvc;

namespace Polaris.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup;

public class PolarisThemeSettingViewComponent(IBrandSettingAppService brandSettingAppService) : AbpViewComponent
{
    private readonly IBrandSettingAppService _brandSettingAppService = brandSettingAppService;

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var brandSetting = await _brandSettingAppService.GetAsync();
        var model = ObjectMapper.Map<BrandSettingDto, ThemeSettingViewModel>(brandSetting);
        return View("~/Pages/SettingManagement/Components/ThemeSettingGroup/Default.cshtml", model);
    }


    public class ThemeSettingViewModel
    {
        [MaxLength(50)]
        [Display(Name = "Site Name")]
        public string? SiteName { get; set; }

        [MaxLength(200)]
        [Display(Name = "Logo Url")]
        public string? LogoUrl { get; set; }

        [MaxLength(200)]
        [Display(Name = "Logo Reverse Url")]
        public string? LogoReverseUrl { get; set; }
    }
}
