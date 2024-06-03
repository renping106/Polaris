namespace Nerd.Abp.DatabaseManagement.Abstractions.Database
{
    public class DbContextChangedEvent
    {
        public DbContextChangedEventType EventType { get; set; }
    }
}
