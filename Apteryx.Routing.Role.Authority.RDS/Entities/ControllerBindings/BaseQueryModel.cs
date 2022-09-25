namespace Apteryx.Routing.Role.Authority.RDS
{
    public abstract class BaseQueryModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页显示的条数
        /// </summary>
        public int Limit { get; set; } = 20;
    }
}
