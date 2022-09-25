namespace Apteryx.Routing.Role.Authority.RDS
{

    public sealed class ResultGroupRouteModel
    {
        /// <summary>
        /// 功能说明
        /// </summary>
        public string CtrlName { get; set; }

        /// <summary>
        /// 路由信息
        /// </summary>
        public IEnumerable<Route> Routes { get; set; }
    }
}
