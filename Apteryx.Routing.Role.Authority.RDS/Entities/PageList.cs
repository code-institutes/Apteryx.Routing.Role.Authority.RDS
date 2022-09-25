using System.Collections.Generic;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public class PageList<T>
    {
        public PageList(long totalCount, IEnumerable<T> data)
        {
            this.TotalCount = totalCount;
            this.Data = data;
        }

        /// <summary>
        /// 数据总数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
