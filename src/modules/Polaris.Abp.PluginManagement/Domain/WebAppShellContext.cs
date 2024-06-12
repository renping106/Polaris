using Microsoft.AspNetCore.Http;

namespace Polaris.Abp.PluginManagement.Domain
{
    public record class WebAppShellContext(
            IServiceProvider Services,
            RequestDelegate RequestDelegate);

}
