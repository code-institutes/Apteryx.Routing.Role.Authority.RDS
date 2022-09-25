using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public sealed class FieldValid
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public IEnumerable<string> Error { get; set; }
    }
}
