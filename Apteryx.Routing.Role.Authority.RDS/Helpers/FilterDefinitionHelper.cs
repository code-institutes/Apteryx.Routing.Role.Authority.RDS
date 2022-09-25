//using Apteryx.MongoDB.Driver.Extend;
//using MongoDB.Driver;

//namespace Apteryx.Routing.Role.Authority.RDS
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public static class FilterDefinitionHelper
//    {
//        public static FilterDefinition<T> And<T>(this FilterDefinition<T> f, FilterDefinition<T> filter) where T : BaseMongoEntity
//        {
//            return f == null ? filter : f & filter;
//        }
//        public static FilterDefinition<T> Or<T>(this FilterDefinition<T> f, FilterDefinition<T> filter) where T : BaseMongoEntity
//        {
//            return f == null ? filter : f | filter;
//        }
//    }
//}
