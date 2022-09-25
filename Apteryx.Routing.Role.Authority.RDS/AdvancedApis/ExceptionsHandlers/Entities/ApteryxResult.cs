namespace Apteryx.Routing.Role.Authority.RDS
{
    public sealed class ApteryxResult : BaseApteryxResult
    {
        public ApteryxResult() { }
        public ApteryxResult(ApteryxCodes code) : this(code, code.ToString())
        {
        }

        public ApteryxResult(ApteryxCodes code, string msg)
        {
            base.code = code;
            base.msg = msg;
        }
    }

    public sealed class ApteryxResult<T> : BaseApteryxResult<T>
        where T : class
    {
        public ApteryxResult() { }

        public ApteryxResult(T result) : this(ApteryxCodes.请求成功, ApteryxCodes.请求成功.ToString(), result)
        {
        }

        public ApteryxResult(ApteryxCodes code) : this(code, code.ToString())
        {
        }

        public ApteryxResult(ApteryxCodes code, string msg) : this(code, msg, null)
        {
        }

        public ApteryxResult(ApteryxCodes code, T result) : this(code, code.ToString(), result)
        {
        }
        public ApteryxResult(ApteryxCodes code, string msg, T result)
        {
            base.code = code;
            base.msg = msg;
            base.result = result;
        }
    }
}
