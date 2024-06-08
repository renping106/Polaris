using Nerd.Abp.Extension.Abstractions.Database;
using Volo.Abp.DependencyInjection;

namespace Nerd.Abp.DatabaseManagement.SqlServer
{
    public class SqlServerLocalDbDatabaseProvider : SqlServerDatabaseProvider, IDatabaseProvider, ITransientDependency
    {
        public override string Name => "LocalDB";

        public override string Key => "LocalDB";

        public override string SampleConnectionString => "Server=(LocalDb)\\MSSQLLocalDB;Database=Nerd;Trusted_Connection=True;TrustServerCertificate=True";

    }
}
