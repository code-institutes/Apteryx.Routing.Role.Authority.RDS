using Apteryx.Routing.Role.Authority.RDS.Data;
using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public class ApteryxDbContext : SugarUnitOfWork
    {
        /// <summary>
        /// 
        /// </summary>
        public DbSet<SystemAccount> SystemAccounts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Role> Roles {get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Route> Routes {get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Log> Logs {get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<RoleRoute> RoleRoutes {get; set; }
    }
}
