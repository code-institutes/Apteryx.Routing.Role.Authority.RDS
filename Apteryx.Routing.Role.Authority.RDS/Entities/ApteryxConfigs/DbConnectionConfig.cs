using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS.Entities.ApteryxConfigs
{
    public class DbConnectionConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbType DbType { get; set; }
    }
}
