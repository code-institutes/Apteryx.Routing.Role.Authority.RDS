using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 编辑路由模型
    /// </summary>
    public sealed class EditRouteModel:AddRouteModel
    {
        /// <summary>
        /// 路由ID
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public long Id { get; set; }
    }
}
