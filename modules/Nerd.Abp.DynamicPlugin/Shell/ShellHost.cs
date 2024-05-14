using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nerd.Abp.DynamicPlugin.Shell
{
    internal record class ShellHost(WebApplication WebApp, RequestDelegate RequestDelegate);
}
