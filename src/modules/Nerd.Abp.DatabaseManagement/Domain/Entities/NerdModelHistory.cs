using System.ComponentModel.DataAnnotations;

namespace Nerd.Abp.DatabaseManagement.Domain.Entities
{
    public class NerdModelHistory
    {
        [Key]
        public int Id { get; set; }
        public required string DbContextFullName { get; set; }
        public required string Snapshot { get; set; }
        public DateTime SnapshotTime { get; set; }
    }
}
