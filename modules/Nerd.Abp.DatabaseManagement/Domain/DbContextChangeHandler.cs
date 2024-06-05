using Nerd.Abp.DatabaseManagement.Abstractions.Database;
using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.SettingManagement;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class DbContextChangeHandler : ILocalEventHandler<DbContextChangedEvent>, ITransientDependency
    {
        private readonly IMigrationManager _migrationManager;
        private readonly ISettingManager _settingManager;

        public DbContextChangeHandler(IMigrationManager migrationManager, ISettingManager settingManager)
        {
            _migrationManager = migrationManager;
            _settingManager = settingManager;
        }

        public async Task HandleEventAsync(DbContextChangedEvent eventData)
        {
            await _migrationManager.MigratePluginSchemaAsync(eventData.DbContextTypes);
        }
    }
}
