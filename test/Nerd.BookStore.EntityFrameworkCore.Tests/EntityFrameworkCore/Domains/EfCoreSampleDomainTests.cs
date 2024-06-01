using Nerd.Abp;
using Nerd.BookStore.Samples;
using Xunit;

namespace Nerd.BookStore.EntityFrameworkCore.Domains;

[Collection(BookStoreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<BookStoreEntityFrameworkCoreTestModule>
{

}
