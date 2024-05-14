using Nerd.BookStore.Samples;
using Xunit;

namespace Nerd.BookStore.EntityFrameworkCore.Applications;

[Collection(BookStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<BookStoreEntityFrameworkCoreTestModule>
{

}
