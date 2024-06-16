using Polaris.Abp.Extension.Abstractions.Database;

namespace Polaris.Abp.DatabaseManagement.Domain.Interfaces;

public interface ICurrentDatabase
{
    IDatabaseProvider Provider { get; }
}
