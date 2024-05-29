using AutoMapper;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Services.Dtos;

namespace Nerd.Abp.DatabaseManagement.Services
{
    internal class PluginManagementAutoMapperProfile : Profile
    {
        public PluginManagementAutoMapperProfile()
        {
            CreateMap<IDatabaseProvider, DatabaseProviderDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.HasConnectionString, opt => opt.MapFrom(src => src.HasConnectionString))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.SampleConnectionString, opt => opt.MapFrom(src => src.SampleConnectionString));
        }
    }
}
