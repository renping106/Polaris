using AutoMapper;
using Nerd.Abp.PluginManagement.Domain.Interfaces;
using Nerd.Abp.PluginManagement.Services.Dtos;

namespace Nerd.Abp.PluginManagement.Services
{
    public class PluginManagementAutoMapperProfile : Profile
    {
        public PluginManagementAutoMapperProfile()
        {
            CreateMap<IPlugInDescriptor, PlugInDescriptorDto>();
        }
    }
}
