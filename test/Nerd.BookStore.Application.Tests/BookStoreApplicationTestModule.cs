using Volo.Abp.Modularity;

namespace Nerd.BookStore;

[DependsOn(
    typeof(BookStoreApplicationModule),
    typeof(BookStoreDomainTestModule)
)]
public class BookStoreApplicationTestModule : AbpModule
{

}
