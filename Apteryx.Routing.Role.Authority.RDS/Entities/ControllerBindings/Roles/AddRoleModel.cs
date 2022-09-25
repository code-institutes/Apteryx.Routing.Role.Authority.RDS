using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 添加角色模型
    /// </summary>
    public class AddRoleModel
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 路由ID
        /// </summary>
        [RequiredItem]
        public IEnumerable<long> RouteIds { get; set; }
    }
}
