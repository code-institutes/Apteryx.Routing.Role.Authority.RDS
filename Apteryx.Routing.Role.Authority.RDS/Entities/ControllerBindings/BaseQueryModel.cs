using System.ComponentModel.DataAnnotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public abstract class BaseQueryModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [Range(1,int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页显示的条数
        /// </summary>
        [Range(1,500)]
        public int Limit { get; set; } = 20;
    }
}
