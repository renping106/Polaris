using Polaris.Abp.ThemeManagement.Localization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace Polaris.Abp.ThemeManagement.Domain;

public class ThemeManagementFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(ThemeManagementFeatures.GroupName,
            L("Feature:ThemeManagementGroup"));

        group.AddFeature(
            ThemeManagementFeatures.Enable,
            "true",
            L("Feature:ThemeManagementEnable"),
            L("Feature:ThemeManagementEnableDescription"),
            new ToggleStringValueType(),
            isAvailableToHost: true);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ThemeManagementResource>(name);
    }
}
