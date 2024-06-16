using System.ComponentModel.DataAnnotations;

namespace Polaris.Abp.DatabaseManagement.Domain.Entities;

public class PolarisModelHistory
{
    [Key]
    public int Id { get; set; }
    public required string DbContextFullName { get; set; }
    public required string Snapshot { get; set; }
    public DateTime SnapshotTime { get; set; }
}
