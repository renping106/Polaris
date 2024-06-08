using Nerd.Abp.ThemeManagement.Localization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace Nerd.Abp.ThemeManagement.Domain
{
    public class ThemeManagementFeatureDefinitionProvider : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            var group = context.AddGroup(ThemeManagementFeatures.GroupName,
                L("Feature:ThemeManagementGroup"));

            group.AddFeature(
                ThemeManagementFeatures.Enable,
                "false",
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
}
