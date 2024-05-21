using AutoMapper;
using Nerd.Abp.DynamicPlugin.Domain.Plugin;
using Nerd.Abp.DynamicPlugin.Services.Dtos;

namespace Nerd.Abp.DynamicPlugin.Services
{
    public class DynamicPluginAutoMapperProfile : Profile
    {
        public DynamicPluginAutoMapperProfile()
        {
            CreateMap<IPlugInDescriptor, PlugInDescriptorDto>();
        }
    }
}
