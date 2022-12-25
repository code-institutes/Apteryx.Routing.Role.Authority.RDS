using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("Apteryx_RoleRoute")]
    public sealed class RoleRoute:BaseEntity
    {
        public long RoleId { get; set; }
        public long RouteId { get; set; }
    }
}
