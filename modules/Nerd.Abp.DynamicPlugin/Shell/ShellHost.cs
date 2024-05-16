using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.DynamicPlugin.Shell
{
    internal record class ShellHost(
            IServiceProvider Services,
            RequestDelegate RequestDelegate,
            Func<WebApplicationBuilder> BuilderInit,
            Type StartupModuleType
        );
}
