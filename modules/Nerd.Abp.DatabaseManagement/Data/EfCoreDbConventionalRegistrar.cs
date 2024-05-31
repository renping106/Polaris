using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Nerd.Abp.DatabaseManagement.Data
{
    public class EfCoreDbConventionalRegistrar : DefaultConventionalRegistrar
    {
        protected override bool IsConventionalRegistrationDisabled(Type type)
        {
            return !typeof(IAbpEfCoreDbContext).IsAssignableFrom(type) || base.IsConventionalRegistrationDisabled(type);
        }

        protected override List<Type> GetExposedServiceTypes(Type type)
        {
            return new List<Type>()
            {
                typeof(IAbpEfCoreDbContext)
            };
        }

        protected override ServiceLifetime? GetDefaultLifeTimeOrNull(Type type)
        {
            return ServiceLifetime.Transient;
        }
    }
}
