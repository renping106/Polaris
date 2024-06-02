using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Shouldly;

namespace Nerd.Abp.DatabaseManagement.Tests
{
    public class SetupAppServiceTest : NerdAbpTestBase<DatabaseManagementTestWithoutSeedModule>
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
        public async Task Host_Should_Be_Initialized()
        {
            //Act
            await _setupAppService.InstallAsync(new SetupInputDto()
            {
                SiteName = "Nerd",
                ConnectionString = "InMemory",
                DatabaseProvider = "InMemory",
                Email = "test@test.com",
                Password = "test"
            });
            var result = _setupAppService.IsInitialized(null);

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Host_Should_Not_Be_Initialized()
        {
            //Act
            var result = _setupAppService.IsInitialized(null);

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Tenant_Should_Not_Be_Initialized()
        {
            //Act
            var result = _setupAppService.IsInitialized(Guid.NewGuid());

            //Assert
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("InMemory", "")]
        [InlineData("Sqlite", "")]
        [InlineData("LocalDB", "")]
        [InlineData("SqlServer", "Wrong string")]
        [InlineData("NotSupported", "Wrong string")]
        public void Host_Should_Fail_To_Install(string databaseProvider, string connectionString)
        {
            //Assert
            var action = async () =>
            {
                await _setupAppService.InstallAsync(new SetupInputDto()
                {
                    SiteName = string.Empty,
                    ConnectionString = connectionString,
                    DatabaseProvider = databaseProvider,
                    Email = string.Empty,
                    Password = string.Empty
                });
            };
            action.ShouldThrow<Exception>();
        }

        [Theory]
        [InlineData("InMemory", "InMemory")]
        [InlineData("Sqlite", "Data Source=Nerd.db;Cache=Shared")]
        [InlineData("LocalDB", "Server=(LocalDb)\\MSSQLLocalDB;Database=Nerd;Trusted_Connection=True;TrustServerCertificate=True")]
        public async Task Host_Should_Install_Successfully(string databaseProvider, string connectionString)
        {
            //Assert
            var action = async () =>
            {
                await _setupAppService.InstallAsync(new SetupInputDto()
                {
                    SiteName = "Nerd",
                    ConnectionString = connectionString,
                    DatabaseProvider = databaseProvider,
                    Email = "test@test.com",
                    Password = "test"
                });
            };
            await action.ShouldNotThrowAsync();
        }
    }
}
