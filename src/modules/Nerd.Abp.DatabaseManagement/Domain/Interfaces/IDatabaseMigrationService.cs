namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface IDatabaseMigrationService
    {
        Task MigrateAsync(string? email = null, string? password = null);
    }
}
