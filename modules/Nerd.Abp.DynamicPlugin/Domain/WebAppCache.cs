using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.DynamicPlugin.Domain
{
    internal record class WebAppCache(
            IServiceProvider Services,
            RequestDelegate RequestDelegate,
            Type StartupModuleTyp,
            Func<WebApplicationBuilder> BuilderInit);
}
