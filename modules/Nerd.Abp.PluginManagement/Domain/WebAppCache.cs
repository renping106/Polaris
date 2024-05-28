using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.PluginManagement.Domain
{
    public record class WebAppCache(
            IServiceProvider Services,
            RequestDelegate RequestDelegate);

}
