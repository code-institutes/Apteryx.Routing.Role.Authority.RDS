using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 添加系统账户模型
    /// </summary>
    public sealed class AddSystemAccountModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "“{0}”必填")]
        public string Name { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "“{0}”必填")]
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "“{0}”必填")]
        public string Password { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public long RoleId { get; set; }
    }
}
