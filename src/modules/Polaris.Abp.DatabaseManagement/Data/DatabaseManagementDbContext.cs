using Microsoft.EntityFrameworkCore;
using Polaris.Abp.DatabaseManagement.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.DatabaseManagement.Data;

public class DatabaseManagementDbContext : AbpDbContext<DatabaseManagementDbContext>
{
    public DbSet<PolarisModelHistory> PolarisModelHistories { get; set; }

    public DatabaseManagementDbContext(DbContextOptions<DatabaseManagementDbContext> options)
        : base(options)
    {

    }
}
