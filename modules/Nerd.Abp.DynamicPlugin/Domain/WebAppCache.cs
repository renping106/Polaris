using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.DynamicPlugin.Domain
{
    public record class WebAppCache(
            IServiceProvider Services,
            RequestDelegate RequestDelegate);

}
