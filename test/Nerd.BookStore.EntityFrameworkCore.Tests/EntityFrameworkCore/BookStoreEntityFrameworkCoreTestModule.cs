using Microsoft.Extensions.DependencyInjection;
using Nerd.Abp.DatabaseManagement.Extensions;
using Nerd.Abp.DatabaseManagement.Sqlite;
using Nerd.Abp.DatabaseManagement.Tests;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Nerd.BookStore.EntityFrameworkCore;

[DependsOn(
    typeof(BookStoreApplicationTestModule),
    typeof(BookStoreEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(DatabaseManagementTestModule),
    typeof(DatabaseManagementSqliteModule)
    )]
public class BookStoreEntityFrameworkCoreTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });
        Configure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
    }
}
