namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface IConfigFileManager
    {
        string? GetDatabaseProvider();
        string? GetConnectionString();
        void SetDatabaseProvider(string databaseProvider);
        void SetConnectionString(string connectionString);
    }
}
