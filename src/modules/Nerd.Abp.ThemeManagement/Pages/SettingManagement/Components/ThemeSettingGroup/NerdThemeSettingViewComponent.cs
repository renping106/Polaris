using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.ThemeManagement.Services.Dtos;
using Nerd.Abp.ThemeManagement.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.AspNetCore.Mvc;

namespace Nerd.Abp.ThemeManagement.Pages.SettingManagement.Components.ThemeSettingGroup
{
    public class NerdThemeSettingViewComponent : AbpViewComponent
    {
        private readonly IBrandSettingAppService _themeSettingAppService;

        public NerdThemeSettingViewComponent(IBrandSettingAppService themeSettingAppService)
        {
            _themeSettingAppService = themeSettingAppService;
        }
        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var themeSetting = await _themeSettingAppService.GetAsync();
            var model = ObjectMapper.Map<ThemeSettingDto, ThemeSettingViewModel>(themeSetting);
            return View("~/Pages/SettingManagement/Components/ThemeSettingGroup/Default.cshtml", model);
        }


        public class ThemeSettingViewModel
        {
            [MaxLength(200)]
            [Display(Name = "Logo Url")]
            public string? LogoUrl { get; set; }

            [MaxLength(200)]
            [Display(Name = "Logo Reverse Url")]
            public string? LogoReverseUrl { get; set; }
        }
    }
}
