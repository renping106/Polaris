using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class DatabaseMigrationService(
    IDataSeeder dataSeeder,
    ICurrentTenant currentTenant,
    ICurrentDatabase currentDatabase,
    IServiceProvider serviceProvider) : IDatabaseMigrationService, ITransientDependency
{
    public ILogger<DatabaseMigrationService> Logger { get; set; } = NullLogger<DatabaseMigrationService>.Instance;

    private readonly IDataSeeder _dataSeeder = dataSeeder;
    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly ICurrentDatabase _currentDatabase = currentDatabase;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task MigrateAsync(string? email = null, string? password = null)
    {
        await MigrateDatabaseSchemaAsync();
        await SeedDataAsync(email, password);
    }

    private async Task MigrateDatabaseSchemaAsync()
    {
        var name = _currentTenant.Id == null ? "host" : _currentTenant.Id + " tenant";
        Logger.LogInformation(
            "Migrating schema for {Name} database...", name);

        if (!_currentDatabase.Provider.IgnoreMigration)
        {
            var migrationManager = _serviceProvider.GetRequiredService<IMigrationManager>();
            await migrationManager.MigrateSchemaAsync();
        }

        Logger.LogInformation("Successfully completed {Name} database migrations.", name);
    }

    private async Task SeedDataAsync(string? email, string? password)
    {
        var name = _currentTenant.Id == null ? "host" : _currentTenant.Id + " tenant";
        Logger.LogInformation("Executing {Name} database seed...", name);

        var seedContext = new DataSeedContext(_currentTenant.Id);

        if (!email.IsNullOrWhiteSpace())
        {
            seedContext.WithProperty("AdminEmail", email);
        }

        if (!password.IsNullOrWhiteSpace())
        {
            seedContext.WithProperty("AdminPassword", password);
        }
        await _dataSeeder.SeedAsync(seedContext);

        Logger.LogInformation("Successfully seeded {Name} database migrations.", name);
    }
}
