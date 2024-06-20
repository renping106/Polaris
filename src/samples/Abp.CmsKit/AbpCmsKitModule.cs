using Volo.Abp.GlobalFeatures;
using Volo.Abp.Modularity;
using Volo.CmsKit;
using Volo.CmsKit.Comments;
using Volo.CmsKit.EntityFrameworkCore;
using Volo.CmsKit.MediaDescriptors;
using Volo.CmsKit.Ratings;
using Volo.CmsKit.Reactions;
using Volo.CmsKit.Tags;
using Volo.CmsKit.Web;
using Volo.CmsKit.Web.Contents;

namespace Abp.Cms;

[DependsOn(
    typeof(CmsKitHttpApiModule),
    typeof(CmsKitDomainModule),
    typeof(CmsKitApplicationModule),
    typeof(CmsKitEntityFrameworkCoreModule),
    typeof(CmsKitWebModule)
)]
public class AbpCmsKitModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        GlobalFeatureManager.Instance.Modules.CmsKit().EnableAll();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureCmsKit();

        Configure<CmsKitContentWidgetOptions>(options =>
        {
            options.AddWidget("Today", "CmsToday", "Format");
        });
    }

    private void ConfigureCmsKit()
    {
        Configure<CmsKitTagOptions>(options =>
        {
            options.EntityTypes.Add(new TagEntityTypeDefiniton("quote"));
        });

        Configure<CmsKitCommentOptions>(options =>
        {
            options.EntityTypes.Add(new CommentEntityTypeDefinition("quote"));
            options.IsRecaptchaEnabled = true;
            options.AllowedExternalUrls = new Dictionary<string, List<string>>
        {
            {
                "quote",
                new List<string>
                {
                    "https://abp.io/"
                }
            }
        };
        });

        Configure<CmsKitMediaOptions>(options =>
        {
            options.EntityTypes.Add(new MediaDescriptorDefinition("quote"));
        });

        Configure<CmsKitReactionOptions>(options =>
        {
            options.EntityTypes.Add(
                new ReactionEntityTypeDefinition("quote",
                reactions: new[]
                {
                    new ReactionDefinition(StandardReactions.ThumbsUp),
                    new ReactionDefinition(StandardReactions.ThumbsDown),
                }));
        });

        Configure<CmsKitRatingOptions>(options =>
        {
            options.EntityTypes.Add(new RatingEntityTypeDefinition("quote"));
        });
    }
}
