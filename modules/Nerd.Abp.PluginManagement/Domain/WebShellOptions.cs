﻿using Microsoft.AspNetCore.Builder;

namespace Nerd.Abp.PluginManagement.Domain
{
    public class WebShellOptions
    {
        public required Type StartupModuleTyp { get; set; }
        public required Func<WebApplicationBuilder> BuilderInit { get; set; }
        public List<Type> SharedServices { get; set; } = new List<Type>();
    }
}