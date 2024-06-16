using Polaris.Abp.ThemeManagement.Domain;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;
using Volo.Abp.Ui.Branding;

namespace Polaris.Abp.ThemeManagement;

public class ThemeBrandingProvider(ISettingProvider settingProvider) : IBrandingProvider, ITransientDependency
{
    private readonly ISettingProvider _settingProvider = settingProvider;

    public string AppName {
        get {
            var appName = _settingProvider.GetOrNullAsync(ThemeManagementSettings.SiteName).Result ?? "";
            return appName;
        }
    }

    public string LogoUrl {
        get {
            var logoUrl = _settingProvider.GetOrNullAsync(ThemeManagementSettings.LogoUrl).Result ?? "";
            return logoUrl;
        }
    }

    public string LogoReverseUrl {
        get {
            var logoUrl = _settingProvider.GetOrNullAsync(ThemeManagementSettings.LogoReverseUrl).Result ?? "";
            return logoUrl;
        }
    }
}
