using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class DbContextChangeHandler : ILocalEventHandler<DbContextChangedEvent>, ITransientDependency
    {
        private readonly IMigrationManager _migrationManager;

        public DbContextChangeHandler(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public async Task HandleEventAsync(DbContextChangedEvent eventData)
        {
            await _migrationManager.MigrateSchemaAsync();
        }
    }
}
