using System.Threading.Tasks;

namespace Nerd.BookStore.Data;

public interface IBookStoreDbSchemaMigrator
{
    Task MigrateAsync();
}
