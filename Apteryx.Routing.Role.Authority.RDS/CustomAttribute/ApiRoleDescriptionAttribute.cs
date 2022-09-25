using System;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 接口描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiRoleDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ApiRoleDescriptionAttribute() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag">唯一标签</param>
        /// <param name="name">API名称</param>
        /// <param name="description">API描述</param>
        /// <param name="isMustHave">是否必有权限</param>
        public ApiRoleDescriptionAttribute(string tag,string name, string? description=null,bool isMustHave = false)
        {
            Tag = tag;
            
            Description = description;
            IsMustHave = isMustHave;
            Name = name;
            //Index = index;
        }
        /// <summary>
        /// 用于
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用于描述API功能
        /// </summary>
        public string? Description { get; set; }

        public bool IsMustHave { get; set; }
        ///// <summary>
        ///// 索引(用于排序)
        ///// </summary>
        //public int Index { get; set; }
    }
}
