using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.ThemeManagement.Services.Dtos;
using Nerd.Abp.ThemeManagement.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc;

namespace Nerd.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup
{
    public class NerdThemeSettingViewComponent : AbpViewComponent
    {
        private readonly IBrandSettingAppService _brandSettingAppService;

        public NerdThemeSettingViewComponent(IBrandSettingAppService brandSettingAppService)
        {
            _brandSettingAppService = brandSettingAppService;
        }
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
}
