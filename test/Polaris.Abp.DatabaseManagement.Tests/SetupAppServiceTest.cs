﻿using Polaris.Abp.DatabaseManagement.Services.Dtos;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;
using Shouldly;
using Volo.Abp.TenantManagement;

namespace Polaris.Abp.DatabaseManagement.Tests;

public class SetupAppServiceTest : PolarisAbpTestBase<DatabaseManagementTestModule>
{
    private readonly ISetupAppService _setupAppService;
    private readonly ITenantAppService _tenantAppService;

    public SetupAppServiceTest()
    {
        _setupAppService = GetRequiredService<ISetupAppService>();
        _tenantAppService = GetRequiredService<ITenantAppService>();
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
            SiteName = "Polaris",
            ConnectionString = "InMemory",
            DatabaseProvider = "InMemory",
            Email = "test@test.com",
            Password = "test"
        }, null);
        var result = _setupAppService.IsInitialized(null);

        //Assert
        result.ShouldBeTrue();
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
    [InlineData("tenanta", true)]
    [InlineData("tenantb", false)]
    public async Task Tenant_Should_Be_Initialized(string tenantName, bool useHostSetting)
    {
        //Create tenant
        var tenant = await _tenantAppService.CreateAsync(new TenantCreateDto()
        {
            Name = tenantName,
            AdminEmailAddress = "test@test.com",
            AdminPassword = "password",
        });

        //Setup tenant
        await _setupAppService.InstallAsync(new SetupInputDto()
        {
            SiteName = "Site Name",
            ConnectionString = tenantName,
            DatabaseProvider = "InMemory",
            Email = "test@test.com",
            Password = "password",
            UseHostSetting = useHostSetting
        }, tenant.Id);

        //Act
        var result = _setupAppService.IsInitialized(tenant.Id);

        //Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("InMemory", "")]
    [InlineData("Sqlite", "")]
    [InlineData("LocalDB", "")]
    [InlineData("SqlServer", "Wrong string")]
    [InlineData("NotSupported", "Wrong string")]
    public async Task Host_Should_Fail_To_Install(string databaseProvider, string connectionString)
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
                Password = string.Empty,
            }, null);
        };
        await action.ShouldThrowAsync<Exception>();
    }

    [Theory]
    [InlineData("InMemory", "InMemory")]
    public async Task Host_Should_Install_Successfully(string databaseProvider, string connectionString)
    {
        //Assert
        var action = async () =>
        {
            await _setupAppService.InstallAsync(new SetupInputDto()
            {
                SiteName = "Polaris",
                ConnectionString = connectionString,
                DatabaseProvider = databaseProvider,
                Email = "test@test.com",
                Password = "test"
            }, null);
        };
        await action.ShouldNotThrowAsync();
    }
}
