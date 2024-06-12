using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Volo.Abp.EntityFrameworkCore;

namespace Polaris.Abp.DatabaseManagement.Data
{
    public static class EfCoreDbContextExtensions
    {
        public static int ExecuteListSqlCommand(this IAbpEfCoreDbContext dbContext, List<string> sqlList)
        {
            int retunInt = 0;
            using (var trans = dbContext.Database.BeginTransaction())
            {
                try
                {
                    sqlList.ForEach(cmd => retunInt += dbContext.Database.ExecuteSqlRaw(cmd));
                    dbContext.Database.CommitTransaction();

                }
                catch (DbException ex)
                {
                    try
                    {
                        dbContext.Database.RollbackTransaction();
                    }
                    catch (DbException)
                    {

                    }

                    throw ex;
                }
            }
            return retunInt;
        }
    }
}
