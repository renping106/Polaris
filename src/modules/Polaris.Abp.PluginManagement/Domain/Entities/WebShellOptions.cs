using Microsoft.AspNetCore.Builder;

namespace Polaris.Abp.PluginManagement.Domain.Entities
{
    public class WebShellOptions
    {
        public required Type StartupModuleTyp { get; set; }
        public required Func<WebApplicationBuilder> InitBuilder { get; set; }
        public List<Type> SharedServices { get; set; } = new List<Type>();
    }
}
