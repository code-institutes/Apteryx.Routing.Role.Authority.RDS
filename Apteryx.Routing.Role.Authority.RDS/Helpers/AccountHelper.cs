using Microsoft.AspNetCore.Http;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public static class AccountHelper
    {
        public static long GetAccountId(this HttpContext context)
        {
            if (context.User.Identity == null)
                throw new Exception("身份为空");
            return long.Parse(context.User.Identity.Name);
        }
    }
}
