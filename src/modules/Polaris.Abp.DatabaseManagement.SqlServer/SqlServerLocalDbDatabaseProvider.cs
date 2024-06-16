using Polaris.Abp.Extension.Abstractions.Database;
using Volo.Abp.DependencyInjection;

namespace Polaris.Abp.DatabaseManagement.SqlServer;

public class SqlServerLocalDbDatabaseProvider : SqlServerDatabaseProvider, IDatabaseProvider, ITransientDependency
{
    public override string Name => "LocalDB";

    public override string Key => "LocalDB";

    public override string SampleConnectionString => "Server=(LocalDb)\\MSSQLLocalDB;Database=Polaris;Trusted_Connection=True;TrustServerCertificate=True";

}
