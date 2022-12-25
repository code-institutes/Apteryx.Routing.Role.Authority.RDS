using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public class TokenValidator : ISecurityTokenValidator
    {
        public bool CanValidateToken { get; private set; } = true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        private readonly string key;
        private readonly string iv;

        public TokenValidator(string aesKey, string aesIv)
        {
            key = aesKey;
            iv = aesIv;
        }

        public bool CanReadToken(string securityToken)
        {
            if (string.IsNullOrWhiteSpace(securityToken))
                return false;
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            string jwtToken = "";
            try
            {
                jwtToken = AES256HandlerApi.Decrypt(securityToken, key, iv);
            }
            catch
            {
                jwtToken = securityToken;
            }
            var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);
            return principal;
        }
    }
}
