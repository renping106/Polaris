using Nerd.Abp.Extension.Abstractions.Database;

namespace Nerd.Abp.DatabaseManagement.Domain.Interfaces
{
    public interface ICurrentDatabase
    {
        IDatabaseProvider Provider { get; }
    }
}
