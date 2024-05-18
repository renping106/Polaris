using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.DynamicPlugin.Shell
{
    internal record class WebAppCache(
            IServiceProvider Services,
            RequestDelegate RequestDelegate,
            Func<bool, ValueTask<WebApplicationBuilder>> BuilderInit);
}
