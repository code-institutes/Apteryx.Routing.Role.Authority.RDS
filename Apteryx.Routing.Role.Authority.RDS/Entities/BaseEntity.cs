using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public abstract class BaseEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = SnowFlakeSingle.Instance.NextId();

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? LastTime { get; set; }
    }
}
