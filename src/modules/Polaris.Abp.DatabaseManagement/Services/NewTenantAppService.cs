using Polaris.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectExtending;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;

namespace Polaris.Abp.DatabaseManagement.Services
{
    [Dependency(ReplaceServices = true)]
    [RemoteService(false)]
    public class NewTenantAppService : TenantAppService
    {
        private readonly ISettingManager _settingManager;

        public NewTenantAppService(
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            IDataSeeder dataSeeder,
            IDistributedEventBus distributedEventBus,
            ILocalEventBus localEventBus,
            ISettingManager settingManager)
            : base(tenantRepository, tenantManager, dataSeeder, distributedEventBus, localEventBus)
        {
            _settingManager = settingManager;
        }

        public override async Task<TenantDto> CreateAsync(TenantCreateDto input)
        {
            var tenant = await TenantManager.CreateAsync(input.Name);
            input.MapExtraPropertiesTo(tenant);

            await TenantRepository.InsertAsync(tenant);
            return ObjectMapper.Map<Tenant, TenantDto>(tenant);
        }

        public override async Task<PagedResultDto<TenantDto>> GetListAsync(GetTenantsInput input)
        {
            var tenants = await base.GetListAsync(input);
            foreach (var item in tenants.Items)
            {
                var dbSetting = await _settingManager.GetOrNullForTenantAsync(DatabaseManagementSettings.DatabaseProvider, item.Id);
                item.SetProperty("Database", dbSetting);
                item.SetProperty("Initilized", dbSetting != null);
            }

            return tenants;
        }
    }
}
