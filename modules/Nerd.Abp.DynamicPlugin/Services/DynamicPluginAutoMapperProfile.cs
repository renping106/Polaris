using AutoMapper;
using Nerd.Abp.DynamicPlugin.Domain.Interfaces;
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
