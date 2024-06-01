using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Shouldly;

namespace Nerd.Abp.DatabaseManagement.Tests
{
    public class SetupAppServiceTest: NerdAbpTestBase<DatabaseManagementTestModule>
    {
        private readonly ISetupAppService _setupAppService;

        public SetupAppServiceTest()
        {
            _setupAppService = GetRequiredService<ISetupAppService>();
        }

        [Fact]
        public void Supported_Database_Providers_Should_Contain_InMemory()
        {
            //Act
            var result = _setupAppService.GetSupportedDatabaseProviders();

            //Assert
            result.Count.ShouldBeGreaterThan(0);
            result.ShouldContain(t => t.Value == "InMemory");
        }

        [Fact]
        public void Host_Should_Be_Initialized()
        {
            //Act
            var result = _setupAppService.IsInitialized(null);

            //Assert
            result.ShouldBeTrue();
        }
    }
}
