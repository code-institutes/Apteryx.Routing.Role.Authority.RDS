using Apteryx.Routing.Role.Authority.RDS.Data;
using SqlSugar;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public class ApteryxDbContext
    {
        public SqlSugarClient DbClient;//用来处理事务多表查询和复杂的操作
        public ApteryxDbContext(ApteryxConfig config)
        {
            var conn = config.DbConnection;

            DbClient = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = conn.ConnectionString,
                DbType = conn.DbType,
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
            DbClient.Aop.OnLogExecuting = (sql, pars) =>
            {
                //Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
                //5.0.8.2 获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                Console.WriteLine(UtilMethods.GetSqlString(conn.DbType, sql, pars));
            };
#endif
        }

        public void InitDataBase()
        {
            DbClient.DbMaintenance.CreateDatabase();
            DbClient.CodeFirst.InitTables(typeof(SystemAccount));
            DbClient.CodeFirst.InitTables(typeof(Role));
            DbClient.CodeFirst.InitTables(typeof(Route));
            DbClient.CodeFirst.InitTables(typeof(Log));
            DbClient.CodeFirst.InitTables(typeof(RoleRoute));
        }

        public DbSet<SystemAccount> SystemAccounts => new DbSet<SystemAccount>(DbClient);

        public DbSet<Role> Roles => new DbSet<Role>(DbClient);

        public DbSet<Route> Routes => new DbSet<Route>(DbClient);

        public DbSet<Log> Logs => new DbSet<Log>(DbClient);

        public DbSet<RoleRoute> RoleRoutes => new DbSet<RoleRoute>(DbClient);
    }
}
