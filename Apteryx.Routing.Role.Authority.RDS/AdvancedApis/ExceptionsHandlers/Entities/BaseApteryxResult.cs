namespace Apteryx.Routing.Role.Authority.RDS
{
    public abstract class BaseApteryxResult : IApteryxResult
    {
        public ApteryxCodes code { get; protected set; } = ApteryxCodes.请求成功;
        public string msg { get; protected set; } = ApteryxCodes.请求成功.ToString();
        public DateTime serverTime => DateTime.Now;
    }

    public abstract class BaseApteryxResult<T> : IApteryxResult<T>
        where T : class
    {
        public T result { get; protected set; } = default(T);
        public ApteryxCodes code { get; protected set; } = ApteryxCodes.请求成功;
        public string msg { get; protected set; } = ApteryxCodes.请求成功.ToString();
        public DateTime serverTime => DateTime.Now;
    }
}
