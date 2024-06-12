using AutoMapper;
using Polaris.Abp.PluginManagement.Domain.Interfaces;
using Polaris.Abp.PluginManagement.Services.Dtos;

namespace Polaris.Abp.PluginManagement.Services
{
    public class PluginManagementAutoMapperProfile : Profile
    {
        public PluginManagementAutoMapperProfile()
        {
            CreateMap<IPlugInDescriptor, PlugInDescriptorDto>();
        }
    }
}
