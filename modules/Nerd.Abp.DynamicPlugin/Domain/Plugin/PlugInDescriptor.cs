﻿using System.Text.Json.Serialization;
using Volo.Abp.Modularity.PlugIns;

namespace Nerd.Abp.DynamicPlugin.Domain.Plugin
{
    internal class PlugInDescriptor : IPlugInDescriptor
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string Version { get; set; } = string.Empty;
        public string AbpVersion { get; set; } = string.Empty;

        [JsonIgnore]
        public IPlugInSource PlugInSource { get; set; }
    }
}
