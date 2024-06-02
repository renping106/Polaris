using AutoMapper;
using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Pages.Setup;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Volo.Abp.TenantManagement;
using static Nerd.Abp.DatabaseManagement.Pages.TenantManagement.Tenants.CreateModalModel;

namespace Nerd.Abp.DatabaseManagement.Services
{
    internal class DatabaseManagementAutoMapperProfile : Profile
    {
        public DatabaseManagementAutoMapperProfile()
        {
            CreateMap<IDatabaseProvider, DatabaseProviderDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.HasConnectionString, opt => opt.MapFrom(src => src.HasConnectionString))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.SampleConnectionString, opt => opt.MapFrom(src => src.SampleConnectionString));

            CreateMap<SetupViewModel, SetupInputDto>()
                .ForMember(dest => dest.SiteName, opt => opt.MapFrom(src => src.SiteName))
                .ForMember(dest => dest.DatabaseProvider, opt => opt.MapFrom(src => src.DatabaseProvider))
                .ForMember(dest => dest.ConnectionString, opt => opt.MapFrom(src => src.ConnectionString))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UseHostSetting, opt => opt.MapFrom(src => src.UseHostSetting));

            CreateMap<TenantInfoModel, TenantCreateDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdminEmailAddress, opt => opt.MapFrom(src => "empty@empty.com"))
                .ForMember(dest => dest.AdminPassword, opt => opt.MapFrom(src => "empty"));
        }
    }
}
