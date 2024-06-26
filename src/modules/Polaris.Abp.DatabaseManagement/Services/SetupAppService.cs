﻿using Microsoft.Extensions.Options;
using Polaris.Abp.DatabaseManagement.Domain;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Polaris.Abp.DatabaseManagement.Services.Dtos;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;
using Polaris.Abp.Extension.Abstractions.Database;
using Polaris.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Polaris.Abp.DatabaseManagement.Services;

[RemoteService(false)]
public class SetupAppService(IDatabaseProviderFactory databaseProviderFactory,
    ITenantDatabaseRepository repository,
    IDatabaseMigrationService migrationService,
    ISettingManager settingManager,
    IConfigFileManager configManager,
    IOptionsMonitor<AbpDbConnectionOptions> options,
    ITenantRepository tenantRepository,
    ITimezoneProvider timezoneProvider) 
    : DatabaseManagementAppServiceBase, ISetupAppService, ITransientDependency
{
    private readonly IConfigFileManager _configManager = configManager;
    private readonly IDatabaseProviderFactory _databaseProviderFactory = databaseProviderFactory;
    private readonly IDatabaseMigrationService _migrationService = migrationService;
    private readonly AbpDbConnectionOptions _options = options.CurrentValue;
    private readonly ITenantDatabaseRepository _repository = repository;
    private readonly ISettingManager _settingManager = settingManager;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ITimezoneProvider _timezoneProvider = timezoneProvider;

    public IReadOnlyList<DatabaseProviderDto> GetSupportedDatabaseProviders()
    {
        var providers = _databaseProviderFactory.GetDatabaseProviders();
        return ObjectMapper.Map<IReadOnlyList<IDatabaseProvider>, IReadOnlyList<DatabaseProviderDto>>(providers);
    }

    public async Task InstallAsync(SetupInputDto input, Guid? tenantId)
    {
        if (!IsInitialized(tenantId))
        {
            if (tenantId.HasValue)
            {
                using (CurrentTenant.Change(tenantId))
                {
                    await SetupTenantAsync(input);
                }
            }
            else
            {
                await SetupHostAsync(input);
            }
        }
    }

    public bool IsInitialized(Guid? tenantId)
    {
        var database = _repository.GetProviderByTenant(tenantId);
        return database != null;
    }

    public async Task<bool> TenantExistsAsync(Guid tenantId)
    {
        var tenant = await _tenantRepository.FindAsync(tenantId);
        return tenant != null;
    }

    public Task<List<NameValue>> GetTimeZonesAsync()
    {
        return Task.FromResult(TimeZoneHelper.GetTimezones(_timezoneProvider.GetWindowsTimezones()));
    }

    private async Task SetupHostAsync(SetupInputDto input)
    {
        try
        {
            _repository.UpsertProviderForTenant(null, input.DatabaseProvider);
            _options.ConnectionStrings.Default = input.ConnectionString;
            await _migrationService.MigrateAsync(input.Email, input.Password);
            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.SiteName, input.SiteName);
            await _settingManager.SetForCurrentTenantAsync(TimingSettingNames.TimeZone, input.TimeZone);
            // save to appSettings.json
            _configManager.SetDatabaseProvider(input.DatabaseProvider);
            _configManager.SetConnectionString(input.ConnectionString);
        }
        catch (Exception)
        {
            _repository.UpsertProviderForTenant(null, null);
            _options.ConnectionStrings.Default = null;
            throw;
        }
    }

    private async Task SetupTenantAsync(SetupInputDto input)
    {
        if (input.UseHostSetting)
        {
            input.DatabaseProvider = _repository.GetProviderByTenant(null)!;
        }

        try
        {
            _repository.UpsertProviderForTenant(CurrentTenant.Id, input.DatabaseProvider);

            using var unitOfWork = UnitOfWorkManager.Begin(true);
            var tenant = await _tenantRepository.GetAsync(CurrentTenant.Id!.Value);
            if (!input.UseHostSetting)
            {
                tenant.SetDefaultConnectionString(input.ConnectionString);
            }
            else
            {
                tenant.RemoveDefaultConnectionString();
            }
            await _tenantRepository.UpdateAsync(tenant);

            await _migrationService.MigrateAsync(input.Email, input.Password);
            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.SiteName, input.SiteName);
            await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseProvider, input.DatabaseProvider);
            await _settingManager.SetForCurrentTenantAsync(TimingSettingNames.TimeZone, input.TimeZone);

            if (input.DatabaseProvider == InMemoryDatabaseProvider.ProviderKey)
            {
                await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DefaultAdminEmail, input.Email);
                await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DefaultAdminPassword, input.Password);
            }

            await unitOfWork.CompleteAsync();
        }
        catch (Exception)
        {
            _repository.UpsertProviderForTenant(CurrentTenant.Id, null);
            throw;
        }

    }
}
