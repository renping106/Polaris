using Microsoft.AspNetCore.Identity;
using Polaris.Abp.DatabaseManagement.Pages.Setup;
using Polaris.Abp.DatabaseManagement.Services.Dtos;
using Polaris.Abp.Extension.Abstractions.Database;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using Volo.Abp.TenantManagement;
using static Polaris.Abp.DatabaseManagement.Pages.TenantManagement.Tenants.CreateModalModel;

namespace Polaris.Abp.DatabaseManagement.Services;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DatabaseProviderToDatabaseProviderDtoMapper : MapperBase<IDatabaseProvider, DatabaseProviderDto>
{
    [MapProperty(nameof(IDatabaseProvider.Key), nameof(DatabaseProviderDto.Value))]
    public override partial DatabaseProviderDto Map(IDatabaseProvider source);

    [MapProperty(nameof(IDatabaseProvider.Key), nameof(DatabaseProviderDto.Value))]
    public override partial void Map(IDatabaseProvider source, DatabaseProviderDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SetupViewModelToSetupInputDtoMapper : MapperBase<SetupViewModel, SetupInputDto>
{
    [MapProperty(nameof(SetupViewModel.Timezone), nameof(SetupInputDto.TimeZone))]
    public override partial SetupInputDto Map(SetupViewModel source);

    [MapProperty(nameof(SetupViewModel.Timezone), nameof(SetupInputDto.TimeZone))]
    public override partial void Map(SetupViewModel source, SetupInputDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TenantInfoModelToTenantCreateDtoMapper : MapperBase<TenantInfoModel, TenantCreateDto>
{
    [MapValue(nameof(TenantCreateDto.AdminEmailAddress), "empty@empty.com")]
    [MapValue(nameof(TenantCreateDto.AdminPassword), "empty")]
    public override partial TenantCreateDto Map(TenantInfoModel source);

    [MapValue(nameof(TenantCreateDto.AdminEmailAddress), "empty@empty.com")]
    [MapValue(nameof(TenantCreateDto.AdminPassword), "empty")]
    public override partial void Map(TenantInfoModel source, TenantCreateDto destination);
}

