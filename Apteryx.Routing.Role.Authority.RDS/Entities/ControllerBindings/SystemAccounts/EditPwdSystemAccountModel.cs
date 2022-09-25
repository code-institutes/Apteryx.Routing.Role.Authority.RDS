using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 修改账户与密码模型
    /// </summary>
    public sealed class EditPwdSystemAccountModel
    {
        /// <summary>
        /// 原邮箱（账户）
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "“{0}”必填")]
        public string OldEmail { get; set; }

        /// <summary>
        /// 原密码
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新邮箱（账户）
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "“{0}”必填")]
        public string NewEmail { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string NewPassword { get; set; }
    }
}
