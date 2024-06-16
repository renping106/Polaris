using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.DatabaseManagement.Domain.Interfaces;

public interface IDbContextsResolver
{
    IEnumerable<IAbpEfCoreDbContext> DbContexts { get; }
}
