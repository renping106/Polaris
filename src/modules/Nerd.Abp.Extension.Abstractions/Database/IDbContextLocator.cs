using System.Reflection;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.Extension.Abstractions.Database
{
    public interface IDbContextLocator
    {
        string GetLocation(IAbpEfCoreDbContext dbContext);
        string GetReferenceLocation(IAbpEfCoreDbContext dbContext, AssemblyName refAssemblyName);
    }
}
