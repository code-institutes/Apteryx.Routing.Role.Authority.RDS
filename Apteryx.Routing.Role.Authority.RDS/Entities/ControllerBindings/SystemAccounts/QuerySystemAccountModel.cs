namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 查询系统账户模型
    /// </summary>
    public sealed class QuerySystemAccountModel : BaseQueryModel
    {
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long? RoleId { get; set; }
    }
}
