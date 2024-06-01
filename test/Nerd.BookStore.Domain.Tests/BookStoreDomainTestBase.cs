using Volo.Abp.Modularity;

namespace Nerd.Abp;

/* Inherit from this class for your domain layer tests. */
public abstract class BookStoreDomainTestBase<TStartupModule> : NerdAbpTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
