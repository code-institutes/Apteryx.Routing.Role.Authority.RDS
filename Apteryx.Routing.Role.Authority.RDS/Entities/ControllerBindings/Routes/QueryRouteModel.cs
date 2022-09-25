namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 查询路由模型
    /// </summary>
    public sealed class QueryRouteModel:BaseQueryModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string? CtrlName { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 是否显示必有路由(权限)?
        /// </summary>
        public bool IsShowMustHave { get; set; } = false;
    }
}
