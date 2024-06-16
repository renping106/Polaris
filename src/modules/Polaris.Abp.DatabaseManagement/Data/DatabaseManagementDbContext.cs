using Microsoft.EntityFrameworkCore;
using Polaris.Abp.DatabaseManagement.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.DatabaseManagement.Data;

public class DatabaseManagementDbContext(DbContextOptions<DatabaseManagementDbContext> options)
    : AbpDbContext<DatabaseManagementDbContext>(options)
{
    public DbSet<PolarisModelHistory> PolarisModelHistories { get; set; }
}
