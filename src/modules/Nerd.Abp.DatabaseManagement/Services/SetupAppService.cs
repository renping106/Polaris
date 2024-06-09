using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Nerd.Abp.Extension.Abstractions.Database;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Nerd.Abp.DatabaseManagement.Services
{
    [RemoteService(false)]
    public class SetupAppService : DatabaseManagementAppServiceBase, ISetupAppService, ITransientDependency
    {
        private readonly IConfigFileManager _configManager;
        private readonly IDatabaseProviderFactory _databaseProviderFactory;
        private readonly IDatabaseMigrationService _migrationService;
        private readonly AbpDbConnectionOptions _options;
        private readonly ITenantDatabaseRepository _repository;
        private readonly ISettingManager _settingManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITimezoneProvider _timezoneProvider;

        public SetupAppService(IDatabaseProviderFactory databaseProviderFactory,
            ITenantDatabaseRepository repository,
            IDatabaseMigrationService migrationService,
            ISettingManager settingManager,
            IConfigFileManager configManager,
            IOptionsMonitor<AbpDbConnectionOptions> options,
            ITenantRepository tenantRepository,
            ITimezoneProvider timezoneProvider)
        {
            _databaseProviderFactory = databaseProviderFactory;
            _repository = repository;
            _migrationService = migrationService;
            _settingManager = settingManager;
            _configManager = configManager;
            _options = options.CurrentValue;
            _tenantRepository = tenantRepository;
            _timezoneProvider = timezoneProvider;
        }

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

        public Task<List<NameValue>> GetTimezonesAsync()
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
                await _settingManager.SetGlobalAsync(TimingSettingNames.TimeZone, input.Timezone);
                // save to appSettings.json
                _configManager.SetDatabaseProvider(input.DatabaseProvider);
                _configManager.SetConnectionString(input.ConnectionString);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
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

                using (var unitOfWork = UnitOfWorkManager.Begin(true))
                {
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
                    await _settingManager.SetForCurrentTenantAsync(TimingSettingNames.TimeZone, input.Timezone);

                    await unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                _repository.UpsertProviderForTenant(CurrentTenant.Id, null);
                throw;
            }

        }
    }
}
