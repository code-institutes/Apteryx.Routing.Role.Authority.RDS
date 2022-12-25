using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS
{
    [SugarTable("Apteryx_Role")]//当和数据库名称不一样可以设置表别名 指定表明
    public sealed class Role:BaseEntity, ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AddTypes AddType { get; set; } = AddTypes.人工;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
