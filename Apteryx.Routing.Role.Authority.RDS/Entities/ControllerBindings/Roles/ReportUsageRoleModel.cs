namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 角色使用率统计模型
    /// </summary>
    public sealed class ReportUsageRoleModel
    {
        /// <summary>
        /// 系统账户详情
        /// </summary>
        public SystemAccount Account { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LogNum { get; set; }
    }
}
