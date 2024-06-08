namespace Nerd.Abp.Extension.Abstractions.Database
{
    public interface IDbContextUpdater
    {
        Task UpdateAsync(DbContextChangedEvent dbContextChangedEvent);
    }
}
