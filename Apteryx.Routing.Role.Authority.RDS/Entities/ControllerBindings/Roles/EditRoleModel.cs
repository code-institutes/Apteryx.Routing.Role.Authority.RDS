using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 编辑角色模型
    /// </summary>
    public sealed class EditRoleModel:AddRoleModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required(ErrorMessage ="“{0}”必填")]
        public long Id { get; set; }
    }
}
