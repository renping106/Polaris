using Nerd.Abp.DatabaseManagement.Domain.Interfaces;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Nerd.Abp.DatabaseManagement.Services
{
    [RemoteService(false)]
    public class SetupAppService : DatabaseManagementAppServiceBase, ISetupAppService, ITransientDependency
    {
        private readonly IDatabaseProviderFactory _databaseProviderFactory;

        public SetupAppService(IDatabaseProviderFactory databaseProviderFactory)
        {
            _databaseProviderFactory = databaseProviderFactory;
        }

        public IReadOnlyList<DatabaseProviderDto> GetSupportedDatabaseProviders()
        {
            var providers = _databaseProviderFactory.GetDatabaseProviders();
            return ObjectMapper.Map<IReadOnlyList<IDatabaseProvider>, IReadOnlyList<DatabaseProviderDto>>(providers);
        }

        public bool IsInitialized(Guid? tenantId)
        {
            return false;
        }
    }
}
