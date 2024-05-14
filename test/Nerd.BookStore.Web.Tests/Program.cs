using Microsoft.AspNetCore.Builder;
using Nerd.BookStore;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<BookStoreWebTestModule>();

public partial class Program
{
}
