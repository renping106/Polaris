using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Nerd.Abp.DatabaseManagement.Abstractions.Database
{
    public interface IDatabaseProvider
    {
        public string Name { get; }
        public string Key { get; }
        public bool HasConnectionString { get; }
        public string SampleConnectionString { get; }
        public bool IgnoreMigration { get; }
        public UnitOfWorkTransactionBehavior UnitOfWorkTransactionBehaviorOption { get; }
        public SequentialGuidType? SequentialGuidTypeOption { get; }
        DbContextOptionsBuilder UseDatabase(AbpDbContextConfigurationContext context);
    }
}
