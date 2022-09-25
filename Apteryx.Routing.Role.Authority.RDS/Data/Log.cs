using SqlSugar;

namespace Apteryx.Routing.Role.Authority.RDS
{
    [SugarTable("Apteryx_Log")]//当和数据库名称不一样可以设置表别名 指定表明
    public sealed class Log : BaseEntity
    {
        public Log() { }
        public Log(long systemAccountId, string mongoCollectionName, ActionMethods actionMethod, string actionName, string? source = null, string? after = null, long? groupId = null)
        {
            this.SystemAccountId = systemAccountId;
            this.MongoCollectionName = mongoCollectionName;
            this.ActionMethod = actionMethod;
            this.ActionName = actionName;
            this.Source = source;
            this.After = after;
            this.GroupId = groupId;
        }
        /// <summary>
        /// 事件组ID
        /// </summary>
        //[BsonRepresentation(BsonType.ObjectId)]
        public long? GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[BsonRepresentation(BsonType.ObjectId)]
        public long SystemAccountId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MongoCollectionName { get; set; }
        /// <summary>
        /// 原
        /// </summary>
        [IsNull]
        public string? Source { get; set; }
        /// <summary>
        /// 之后
        /// </summary>
        [IsNull]
        public string? After { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ActionMethods ActionMethod { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActionName { get; set; }
    }
}
