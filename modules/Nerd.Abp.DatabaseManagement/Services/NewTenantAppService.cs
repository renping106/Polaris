using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;

namespace Nerd.Abp.DatabaseManagement.Services
{
    [Dependency(ReplaceServices = true)]
    [RemoteService(false)]
    public class NewTenantAppService : TenantAppService
    {
        public NewTenantAppService(
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            IDataSeeder dataSeeder,
            IDistributedEventBus distributedEventBus,
            ILocalEventBus localEventBus)
            : base(tenantRepository, tenantManager, dataSeeder, distributedEventBus, localEventBus)
        {

        }

        public override async Task<TenantDto> CreateAsync(TenantCreateDto input)
        {
            var tenant = await TenantManager.CreateAsync(input.Name);
            input.MapExtraPropertiesTo(tenant);

            await TenantRepository.InsertAsync(tenant);
            return ObjectMapper.Map<Tenant, TenantDto>(tenant);
        }
    }
}
