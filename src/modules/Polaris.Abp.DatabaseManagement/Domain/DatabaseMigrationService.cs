using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Polaris.Abp.DatabaseManagement.Domain
{
    internal class DatabaseMigrationService : IDatabaseMigrationService, ITransientDependency
    {
        public ILogger<DatabaseMigrationService> Logger { get; set; }

        private readonly IDataSeeder _dataSeeder;
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentDatabase _currentDatabase;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseMigrationService(
            IDataSeeder dataSeeder,
            ICurrentTenant currentTenant,
            ICurrentDatabase currentDatabase,
            IServiceProvider serviceProvider)
        {
            _dataSeeder = dataSeeder;
            _currentTenant = currentTenant;

            Logger = NullLogger<DatabaseMigrationService>.Instance;
            _currentDatabase = currentDatabase;
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync(string? email = null, string? password = null)
        {
            await MigrateDatabaseSchemaAsync();
            await SeedDataAsync(email, password);
        }

        private async Task MigrateDatabaseSchemaAsync()
        {
            var name = _currentTenant == null ? "host" : _currentTenant.Id + " tenant";
            Logger.LogInformation(
                "Migrating schema for {name} database...", name);

            if (!_currentDatabase.Provider.IgnoreMigration)
            {
                var migrationManager = _serviceProvider.GetRequiredService<IMigrationManager>();
                await migrationManager.MigrateSchemaAsync();
            }

            Logger.LogInformation("Successfully completed {name} database migrations.", name);
        }

        private async Task SeedDataAsync(string? email, string? password)
        {
            var name = _currentTenant == null ? "host" : _currentTenant.Id + " tenant";
            Logger.LogInformation("Executing {name} database seed...", name);

            var seedContext = new DataSeedContext(_currentTenant?.Id);

            if (!email.IsNullOrWhiteSpace())
            {
                seedContext.WithProperty("AdminEmail", email);
            }

            if (!password.IsNullOrWhiteSpace())
            {
                seedContext.WithProperty("AdminPassword", password);
            }
            await _dataSeeder.SeedAsync(seedContext);

            Logger.LogInformation("Successfully seeded {name} database migrations.", name);
        }
    }
}
