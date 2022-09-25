using Apteryx.Routing.Role.Authority.RDS.Entities.ApteryxConfigs;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public sealed class ApteryxConfig
    {
        /// <summary>
        /// 认知方案(缺省值：apteryx)
        /// </summary>
        public string AuthenticationScheme { get; set; } = "apteryx";
        /// <summary>
        /// Token配置
        /// </summary>
        public TokenSetting? TokenConfig { get; set; }

        /// <summary>
        /// 是否开启加密Token
        /// </summary>
        public bool IsSecurityToken { get; set; } = false;

        /// <summary>
        /// AES加密配置
        /// </summary>
        public AES256Setting? AESConfig { get; set; }

        /// <summary>
        /// MongoDB连接配置
        /// </summary>
        public DbConnectionConfig? DbConnection { get; set; }
    }
}
