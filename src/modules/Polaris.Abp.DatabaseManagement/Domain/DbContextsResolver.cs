using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Polaris.Abp.DatabaseManagement.Domain.Entities;
using Polaris.Abp.DatabaseManagement.Domain.Interfaces;
using System.Reflection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace Polaris.Abp.DatabaseManagement.Domain;

internal class DbContextsResolver(ICurrentTenant currentTenant,
    IEnumerable<IAbpEfCoreDbContext> dbContexts) : IDbContextsResolver, ITransientDependency
{

    public IEnumerable<IAbpEfCoreDbContext> DbContexts {
        get {
            var filteredDbContexts = new List<IAbpEfCoreDbContext>(_dbContexts);
            var replaceDbContextAttributes = _dbContexts.SelectMany(t =>
                    t.GetType().GetCustomAttributes<ReplaceDbContextAttribute>());
            foreach (var item in replaceDbContextAttributes)
            {
                if (item != null)
                {
                    var replacedDbContexts = item.ReplacedDbContextTypes.Select(t => t.Type);

                    foreach (var replaced in replacedDbContexts)
                    {
                        filteredDbContexts.RemoveAll(t => t.GetType().IsAssignableTo(replaced)
                        && !t.GetType().GetCustomAttributes<ReplaceDbContextAttribute>().Any());
                    }
                }
            }

            if (_currentTenant.Id.HasValue)
            {
                filteredDbContexts.RemoveAll(t => t.GetType().GetCustomAttribute<IgnoreMultiTenancyAttribute>() != null);
            }

            return filteredDbContexts.OrderBy(t => t.GetService<IDesignTimeModel>().Model
                            .FindEntityType(typeof(PolarisModelHistory)) != null ? 0 : 1);
        }
    }

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IEnumerable<IAbpEfCoreDbContext> _dbContexts = dbContexts;

}
