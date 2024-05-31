using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface IDbContextsResolver
    {
        IEnumerable<IAbpEfCoreDbContext> DbContexts { get; }
    }
}
