using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS
{
    [SugarTable("Apteryx_Route")]//当和数据库名称不一样可以设置表别名 指定表明
    public sealed class Route : BaseEntity, ICloneable
    {
        public string CtrlName { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public AddTypes AddType { get; set; } = AddTypes.人工;
        public string Tag { get; set; }
        public string Name { get; set; }
        [IsNull]
        public string Description { get; set; }
        public bool IsMustHave { get; set; }
        public string CtrlFullName { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
