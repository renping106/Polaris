using Nerd.Abp.NewFireTheme.Domain;
using Volo.Abp.SettingManagement;
using Volo.Abp.Ui.Branding;

namespace Nerd.Abp.NewFireTheme
{
    public class NerdNewFireBrandingProvider : IBrandingProvider
    {
        private readonly ISettingManager _settingManager;

        public NerdNewFireBrandingProvider(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        public string AppName => GetAppName();

        public string LogoUrl
        {
            get
            {
                var logoUrl = _settingManager.GetOrNullForCurrentTenantAsync(NerdNewFireThemeSettings.LogoUrl).Result;
                return logoUrl.IsNullOrWhiteSpace() ? "/themes/newfire/images/logo/logo-dark-thumbnail.png" : logoUrl;
            }
        }

        public string LogoReverseUrl
        {
            get
            {
                var logoUrl = _settingManager.GetOrNullForCurrentTenantAsync(NerdNewFireThemeSettings.LogoReverseUrl).Result;
                return logoUrl.IsNullOrWhiteSpace() ? "/themes/newfire/images/logo/logo-light-thumbnail.png" : logoUrl;
            }
        }

        private string GetAppName()
        {
            var appName = string.Empty;
            try
            {
                appName = _settingManager.GetOrNullForCurrentTenantAsync("Nerd.Abp.DatabaseManagement.SiteName").Result;
            }
            catch (Exception)
            {
                appName = _settingManager.GetOrNullForCurrentTenantAsync(NerdNewFireThemeSettings.SiteName).Result;
            }
            return appName;
        }
    }
}
