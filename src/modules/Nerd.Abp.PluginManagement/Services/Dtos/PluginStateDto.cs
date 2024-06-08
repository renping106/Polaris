using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nerd.Abp.PluginManagement.Services.Dtos
{
    public class PluginStateDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
