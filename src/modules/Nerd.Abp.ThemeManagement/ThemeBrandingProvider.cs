using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;
using Volo.Abp.Ui.Branding;

namespace Nerd.Abp.ThemeManagement
{
    public class ThemeBrandingProvider : IBrandingProvider, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public ThemeBrandingProvider(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public string AppName
        {
            get
            {
                var appName = _settingManager.GetOrNullForCurrentTenantAsync("Nerd.Abp.DatabaseManagement.SiteName").Result;
                return appName;
            }
        }

        public string LogoUrl
        {
            get
            {
                var logoUrl = _settingManager.GetOrNullForCurrentTenantAsync(ThemeManagementSettings.LogoUrl).Result;
                return logoUrl;
            }
        }

        public string LogoReverseUrl
        {
            get
            {
                var logoUrl = _settingManager.GetOrNullForCurrentTenantAsync(ThemeManagementSettings.LogoReverseUrl).Result;
                return logoUrl;
            }
        }
    }
}
