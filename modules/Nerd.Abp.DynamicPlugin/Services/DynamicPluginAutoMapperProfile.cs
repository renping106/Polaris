using AutoMapper;
using Nerd.Abp.DynamicPlugin.Services.Dtos;
using Nerd.Abp.DynamicPlugin.Shell;

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
