using Microsoft.Extensions.Options;
using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Nerd.Abp.ThemeManagement.Domain;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;

namespace Nerd.Abp.DatabaseManagement.Services
{
    [RemoteService(false)]
    public class SetupAppService : DatabaseManagementAppServiceBase, ISetupAppService, ITransientDependency
    {
        private readonly IDatabaseProviderFactory _databaseProviderFactory;
        private readonly ITenantDatabaseRepository _repository;
        private readonly IDatabaseMigrationService _migrationService;
        private readonly ISettingManager _settingManager;
        private readonly IConfigFileManager _configManager;
        private readonly AbpDbConnectionOptions _options;
        private readonly ITenantRepository _tenantRepository;

        public SetupAppService(IDatabaseProviderFactory databaseProviderFactory,
            ITenantDatabaseRepository repository,
            IDatabaseMigrationService migrationService,
            ISettingManager settingManager,
            IConfigFileManager configManager,
            IOptionsMonitor<AbpDbConnectionOptions> options,
            ITenantRepository tenantRepository)
        {
            _databaseProviderFactory = databaseProviderFactory;
            _repository = repository;
            _migrationService = migrationService;
            _settingManager = settingManager;
            _configManager = configManager;
            _options = options.CurrentValue;
            _tenantRepository = tenantRepository;
        }

        public IReadOnlyList<DatabaseProviderDto> GetSupportedDatabaseProviders()
        {
            var providers = _databaseProviderFactory.GetDatabaseProviders();
            return ObjectMapper.Map<IReadOnlyList<IDatabaseProvider>, IReadOnlyList<DatabaseProviderDto>>(providers);
        }

        public async Task InstallAsync(SetupInputDto input)
        {
            var tenantId = CurrentTenant.Id;
            if (!IsInitialized(tenantId))
            {
                if (tenantId.HasValue)
                {
                    await SetupTenant(input);
                }
                else
                {
                    await SetupHost(input);
                }
            }
        }

        public bool IsInitialized(Guid? tenantId)
        {
            var database = _repository.GetProviderByTenant(CurrentTenant.Id);
            return database != null;
        }

        private async Task SetupHost(SetupInputDto input)
        {
            try
            {
                _repository.UpsertProviderForTenant(null, input.DatabaseProvider);
                _options.ConnectionStrings.Default = input.ConnectionString;
                await _migrationService.MigrateAsync(input.Email, input.Password);
                await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.SiteName, input.SiteName);
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

        private async Task SetupTenant(SetupInputDto input)
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

                    await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.DatabaseProvider, input.DatabaseProvider);
                    await unitOfWork.CompleteAsync();
                }

                await _migrationService.MigrateAsync(input.Email, input.Password);
                await _settingManager.SetForCurrentTenantAsync(DatabaseManagementSettings.SiteName, input.SiteName);
            }
            catch (Exception)
            {
                using (var unitOfWork = UnitOfWorkManager.Begin(true))
                {
                    var tenant = await _tenantRepository.GetAsync(CurrentTenant.Id!.Value);
                    tenant.RemoveDefaultConnectionString();
                    await _tenantRepository.UpdateAsync(tenant);
                }
                _repository.UpsertProviderForTenant(CurrentTenant.Id, null);
                throw;
            }

        }
    }
}
