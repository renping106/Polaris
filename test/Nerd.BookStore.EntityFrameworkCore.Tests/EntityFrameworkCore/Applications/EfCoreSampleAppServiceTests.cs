using Nerd.BookStore.Samples;
using Xunit;
using Nerd.Abp;

namespace Nerd.BookStore.EntityFrameworkCore.Applications;

[Collection(BookStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<BookStoreEntityFrameworkCoreTestModule>
{

}
