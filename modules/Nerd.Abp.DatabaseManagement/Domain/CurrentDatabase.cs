using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.DatabaseManagement.Domain
{
    internal class CurrentDatabase : ICurrentDatabase, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IDatabaseProviderFactory _databaseProviderFactory;

        public CurrentDatabase(ICurrentTenant currentTenant, IDatabaseProviderFactory databaseProviderFactory)
        {
            _currentTenant = currentTenant;
            _databaseProviderFactory = databaseProviderFactory;
        }

        public IDatabaseProvider Provider => _databaseProviderFactory.GetDatabaseProvider("InMemory");
    }
}
