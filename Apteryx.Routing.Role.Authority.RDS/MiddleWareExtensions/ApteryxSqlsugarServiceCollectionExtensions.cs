using Apteryx.Routing.Role.Authority.RDS.Entities.ApteryxConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Data.Common;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public static class ApteryxSqlsugarServiceCollectionExtensions
    {
        public static void AddApteryxSqlsugar(this IServiceCollection services, DbConnectionConfig conn)
        {
            var sugar = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = conn.ConnectionString,
                DbType = DbType.PostgreSQL,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        // int?  decimal?这种 isnullable=true
                        if (c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            p.IsNullable = true;
                        }
                        else if (c.PropertyType == typeof(string) && c.GetCustomAttributes(true).Any(a => a.GetType() == typeof(IsNullAttribute)))
                        {
                            p.IsNullable = true;
                        }
                    }
                }
            });
#if DEBUG
            sugar.Aop.OnLogExecuting = (sql, pars) =>
            {
                //Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
                //5.0.8.2 获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                Console.WriteLine(UtilMethods.GetSqlString(conn.DbType, sql, pars));
            };
#endif
            SnowFlakeSingle.WorkId = 31;
            sugar.DbMaintenance.CreateDatabase();
            sugar.CodeFirst.InitTables(typeof(SystemAccount));
            sugar.CodeFirst.InitTables(typeof(Role));
            sugar.CodeFirst.InitTables(typeof(Route));
            sugar.CodeFirst.InitTables(typeof(Log));
            sugar.CodeFirst.InitTables(typeof(RoleRoute));
            services.AddSingleton<ISqlSugarClient>(sugar);

            ISugarUnitOfWork<ApteryxDbContext> context = new SugarUnitOfWork<ApteryxDbContext>(sugar);
            services.AddSingleton<ISugarUnitOfWork<ApteryxDbContext>>(context);
        }
    }
}
