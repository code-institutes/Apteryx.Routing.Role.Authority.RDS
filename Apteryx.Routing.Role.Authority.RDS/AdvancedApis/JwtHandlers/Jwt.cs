using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public sealed class Jwt<T>
    {
        public Jwt(JwtSecurityToken securityToken) : this(securityToken, default(T)) { }
        public Jwt(JwtSecurityToken securityToken,string aesKey,string aesIv):this(securityToken, aesKey, aesIv, default(T)) { }
        public Jwt(JwtSecurityToken securityToken, T? obj)
        {
            this.access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            this.valid_to = securityToken.ValidTo;
            this.AppendInfo = obj;
        }
        public Jwt(JwtSecurityToken accessSecurityToken, string aesKey, string aesIv,T? obj)
        {
            this.access_token = AES256HandlerApi.Encrypt(new JwtSecurityTokenHandler().WriteToken(accessSecurityToken), aesKey, aesIv);
            this.valid_to = accessSecurityToken.ValidTo;
            this.AppendInfo = obj;
        }
        /// <summary>
        /// 账户信息
        /// </summary>
        public T? AppendInfo { get; set; }

        /// <summary>
        /// 访问凭证(JWT协议)
        /// </summary>
        public string access_token {get; set;}
        /// <summary>
        /// 前缀
        /// </summary>
        public string token_type => JwtBearerDefaults.AuthenticationScheme;
        /// <summary>
        /// 有效截止时间
        /// </summary>
        public DateTime valid_to { get; set; }
    }
}
