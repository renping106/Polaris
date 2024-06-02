using Shouldly;
using Volo.Abp.TenantManagement;

namespace Nerd.Abp.DatabaseManagement.Tests
{
    public class NewTenantAppServiceTest : NerdAbpTestBase<DatabaseManagementTestModule>
    {
        private readonly ITenantAppService _tenantAppService;

        public NewTenantAppServiceTest()
        {
            _tenantAppService = GetRequiredService<ITenantAppService>();
        }

        [Theory]
        [InlineData("Tenant1")]
        public async Task Tenant_Should_Install_Successfully(string tenantName)
        {
            //Act
            var tenant = await _tenantAppService.CreateAsync(new TenantCreateDto()
            {
                Name = tenantName,
                AdminEmailAddress = "test@test.com",
                AdminPassword = "password",
            });

            //Assert
            tenant.ShouldNotBeNull();
        }
    }
}
