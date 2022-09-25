using SqlSugar;
using System.Text.Json.Serialization;

namespace Apteryx.Routing.Role.Authority.RDS
{
    [SugarTable("Apteryx_SystemAccount")]//当和数据库名称不一样可以设置表别名 指定表明
    public sealed class SystemAccount: BaseEntity, ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        [IsNull]
        public string Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        //[BsonRepresentation(BsonType.ObjectId)]
        public long? RoleId { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        [JsonIgnore]
        public bool IsSuper { get; set; } = false;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
