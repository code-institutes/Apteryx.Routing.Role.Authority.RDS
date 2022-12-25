using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS.Data
{
    //可以直接用SimpleClient也可以扩展一个自个的类 
    //推荐直接用 SimpleClient 
    //为了照顾需要扩展的朋友，我们就来扩展一个SimpleClient，取名叫DbSet
    public class DbSet<T> : SimpleClient<T> where T : class, new()
    {
        public DbSet(ISqlSugarClient context = null) : base(context)
        {
            base.Context = context;
        }
        //SimpleClient中的方法满足不了你，你可以扩展自已的方法

        ///// <summary>
        ///// 返回可迭代的
        ///// </summary>
        ///// <returns></returns>
        //public ISugarQueryable<T> AsQueryable()
        //{
        //    return Context.Queryable<T>();
        //}

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Sql(string sql)
        {
            return Context.SqlQueryable<T>(sql);
        }

        internal Task<int> CountAsync()
        {
            return Context.Queryable<T>().CountAsync();
        }
    }
}
