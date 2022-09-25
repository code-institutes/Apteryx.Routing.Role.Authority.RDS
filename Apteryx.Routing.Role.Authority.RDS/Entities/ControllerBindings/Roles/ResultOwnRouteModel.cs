
namespace Apteryx.Routing.Role.Authority.RDS
{
    public sealed class ResultOwnRouteModel
    {
        /// <summary>
        /// 角色详情
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// 权限详情
        /// </summary>
        public IEnumerable<GroupRouteModel> GroupRoutes { get; set; }
    }
    public sealed class GroupRouteModel
    {
        /// <summary>
        /// 路由标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 路由信息
        /// </summary>
        public IEnumerable<RouteInfoModel> RouteInfo { get; set; }
    }
    public sealed class RouteInfoModel
    {
        /// <summary>
        /// 路由详情
        /// </summary>
        public Route Route { get; set; }

        /// <summary>
        /// 是否拥有此权限
        /// </summary>
        public bool IsOwn { get; set; }
    }
}
