namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 
    /// </summary>
    public class ResultSystemAccountRoleModel
    {
        public ResultSystemAccountRoleModel(SystemAccount systemAccount,Role role)
        {
            this.Account = systemAccount;
            this.Role = role;
        }
        /// <summary>
        /// 系统账户详情
        /// </summary>
        public SystemAccount Account { get; set; }

        /// <summary>
        /// 角色详情
        /// </summary>
        public Role Role { get; set; }
    }
}
