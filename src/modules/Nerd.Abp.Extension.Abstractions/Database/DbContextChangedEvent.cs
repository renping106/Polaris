namespace Nerd.Abp.Extension.Abstractions.Database
{
    public class DbContextChangedEvent
    {
        public required IReadOnlyList<Type> DbContextTypes { get; set; }
    }
}
