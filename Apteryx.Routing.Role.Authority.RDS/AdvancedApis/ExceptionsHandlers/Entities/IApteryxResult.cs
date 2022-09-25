namespace Apteryx.Routing.Role.Authority.RDS
{
    public interface IApteryxResult
    {
        ApteryxCodes code { get; }
        string msg { get; }
    }

    public interface IApteryxResult<out T> : IApteryxResult
        where T : class
    {
        T result { get; }
    }
}
