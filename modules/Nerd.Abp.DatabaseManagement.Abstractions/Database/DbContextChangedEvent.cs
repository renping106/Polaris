namespace Nerd.Abp.DatabaseManagement.Abstractions.Database
{
    public class DbContextChangedEvent
    {
        public required IReadOnlyList<Type> DbContextTypes { get; set; }
    }
}
