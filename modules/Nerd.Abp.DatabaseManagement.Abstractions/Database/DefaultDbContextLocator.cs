using System.Reflection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Abstractions.Database
{
    public class DefaultDbContextLocator : IDbContextLocator, ITransientDependency
    {
        public virtual string GetLocation(IAbpEfCoreDbContext dbContext)
        {
            return dbContext.GetType().Assembly.Location;
        }

        public virtual string GetReferenceLocation(IAbpEfCoreDbContext dbContext, AssemblyName refAssemblyName)
        {
            var assembly = Assembly.Load(refAssemblyName);
            return assembly.Location;
        }
    }
}
