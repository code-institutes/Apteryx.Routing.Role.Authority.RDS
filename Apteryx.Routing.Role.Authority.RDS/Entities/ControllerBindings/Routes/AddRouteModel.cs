using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 添加路由模型
    /// </summary>
    public class AddRouteModel
    {
        /// <summary>
        /// 功能说明
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string CtrlName { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string Method { get; set; }

        /// <summary>
        /// 接口描述
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string Description { get; set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string Path { get; set; }
    }
}
