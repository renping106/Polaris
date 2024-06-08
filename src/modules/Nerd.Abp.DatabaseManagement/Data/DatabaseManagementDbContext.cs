using Microsoft.EntityFrameworkCore;
using Nerd.Abp.DatabaseManagement.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Data
{
    public class DatabaseManagementDbContext : AbpDbContext<DatabaseManagementDbContext>
    {
        public DbSet<NerdModelHistory> NerdModelHistories { get; set; }

        public DatabaseManagementDbContext(DbContextOptions<DatabaseManagementDbContext> options)
            : base(options)
        {

        }
    }
}
