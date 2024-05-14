using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Nerd.BookStore.Web;

[Dependency(ReplaceServices = true)]
public class BookStoreBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "BookStore";
}
