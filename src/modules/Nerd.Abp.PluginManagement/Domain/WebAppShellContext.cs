using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.PluginManagement.Domain
{
    public record class WebAppShellContext(
            IServiceProvider Services,
            RequestDelegate RequestDelegate);

}
