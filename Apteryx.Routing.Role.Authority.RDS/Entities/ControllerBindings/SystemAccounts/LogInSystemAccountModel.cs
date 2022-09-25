using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 登录模型
    /// </summary>
    public sealed class LogInSystemAccountModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "“{0}”必填")]
        public string Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "“{0}”必填")]
        public string Password { get; set; }
    }
}
