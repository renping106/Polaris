using Microsoft.EntityFrameworkCore;
using Nerd.Abp.DatabaseManagement.Domain.Entities;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Data
{
    [ConnectionStringName("Default")]
    public class DatabaseManagementDbContext : AbpDbContext<DatabaseManagementDbContext>
    {
        public DbSet<ModelHistory> ModelHistories { get; set; }

        public DatabaseManagementDbContext(DbContextOptions<DatabaseManagementDbContext> options)
            : base(options)
        {

        }
    }
}
