namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// Restful API返回信息
    /// </summary>
    public static class ApteryxResultApi
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        /// <returns>返回 CGIObjectResult 类型</returns>
        public static ApteryxResult Susuccessful() => new ApteryxResult();
        /// <summary>
        /// 请求成功
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="result">结果</param>
        /// <returns>返回 CGIObjectResult&lt;T&gt; 类型</returns>
        public static ApteryxResult<T> Susuccessful<T>(T result) where T : class => new ApteryxResult<T>(result);
        /// <summary>
        /// 请求异常
        /// </summary>
        /// <param name="code">异常代码</param>
        /// <returns>返回 CGIObjectResult 类型</returns>
        public static ApteryxResult Fail(ApteryxCodes code) => new ApteryxResult(code);
        /// <summary>
        /// 请求异常
        /// </summary>
        /// <param name="code">异常代码</param>
        /// <param name="msg">异常说明</param>
        /// <returns>返回 CGIObjectResult 类型</returns>
        public static ApteryxResult Fail(ApteryxCodes code, string msg) => new ApteryxResult(code, msg);
        /// <summary>
        /// 请求异常
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="code">异常代码</param>
        /// <param name="result">结果</param>
        /// <returns>返回 CGIObjectResult&lt;T&gt; 类型</returns>
        public static ApteryxResult<T> Fail<T>(ApteryxCodes code, T result) where T : class => new ApteryxResult<T>(code, result);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="code">异常代码</param>
        /// <param name="msg">异常说明</param>
        /// <param name="result">结果</param>
        /// <returns>返回 CGIObjectResult&lt;T&gt; 类型</returns>
        public static ApteryxResult<T> Fail<T>(ApteryxCodes code, string msg, T result) where T : class => new ApteryxResult<T>(code, msg, result);
    }
}
