using Volo.Abp.Modularity;

namespace Nerd.Abp;

public abstract class BookStoreApplicationTestBase<TStartupModule> : NerdAbpTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
