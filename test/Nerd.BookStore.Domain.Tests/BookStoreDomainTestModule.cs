using Nerd.Abp;
using Nerd.BookStore;
using Volo.Abp.Modularity;

namespace Nerd.BookStore;

[DependsOn(
    typeof(BookStoreDomainModule),
    typeof(BookStoreTestBaseModule)
)]
public class BookStoreDomainTestModule : AbpModule
{

}
