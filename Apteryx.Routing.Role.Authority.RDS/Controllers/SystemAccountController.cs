using apteryx.common.extend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Text;

namespace Apteryx.Routing.Role.Authority.RDS.Controllers
{
    [Authorize(AuthenticationSchemes = "apteryx")]
    [SwaggerTag("系统账户服务")]
    [Route("cgi-bin/apteryx/system/account")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "zh1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    [SwaggerResponse((int)ApteryxCodes.字段验证未通过, null, typeof(ApteryxResult<IEnumerable<FieldValid>>))]
    public class SystemAccountController : Controller
    {
        private readonly ApteryxDbContext _db;

        private static object _lock = new object();

        public readonly ApteryxConfig _jwtConfig;
        public SystemAccountController(ApteryxConfig jwtConfig, ApteryxDbContext context)
        {
            _db = context;
            _jwtConfig = jwtConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("log-in")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "账户登陆",
            Description = "提交账号和密码换取访问令牌",
            OperationId = "LogIn",
            Tags = new[] { "SystemAccount" }
        )]
        [SwaggerResponse(200, null, typeof(ApteryxResult<Jwt<ResultSystemAccountRoleModel>>))]
        [SwaggerResponse((int)ApteryxCodes.账号或密码错误, null, typeof(ApteryxResult))]
        public async Task<IActionResult> LogIn([FromBody] LogInSystemAccountModel model)
        {
            //创建账户
            var act = _db.SystemAccounts.AsQueryable();
            if (!act.Any())
                await _db.SystemAccounts.InsertAsync(new SystemAccount()
                {
                    Email = "wyspaces@outlook.com",
                    Password = "admin1234".ToSHA1(),
                    IsSuper = true
                });

            var pwd = model.Password.ToSHA1();
            var account = await _db.SystemAccounts.AsQueryable().FirstAsync(f => f.Email == model.Email && f.Password == pwd);
            if (account == null)
            {
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.账号或密码错误));
            }

            var role = await _db.Roles.GetFirstAsync(f => f.Id == account.RoleId);

            var token = new JwtBuilder()
                .AddAudience(_jwtConfig.TokenConfig.Audience)
                .AddClaim(ClaimTypes.Name, account.Id.ToString())
                .AddSubject(Guid.NewGuid().ToString())
                .AddExpiry(_jwtConfig.TokenConfig.Expires)
                .AddIssuer(_jwtConfig.TokenConfig.Issuer)
                .AddSecurityKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.TokenConfig.Key)))
                .Build();
            if (_jwtConfig.IsSecurityToken)
            {
                var aesConfig = _jwtConfig.AESConfig;
                return Ok(ApteryxResultApi.Susuccessful(new Jwt<ResultSystemAccountRoleModel>(token, aesConfig.Key, aesConfig.IV, new ResultSystemAccountRoleModel(account, role))));
            }
            return Ok(ApteryxResultApi.Susuccessful(new Jwt<ResultSystemAccountRoleModel>(token, new ResultSystemAccountRoleModel(account, role))));
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "添加账户",
            OperationId = "Post",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("A", "添加")]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Post([FromBody] AddSystemAccountModel model)
        {
            var email = model.Email?.Trim();
            var pwd = model.Password.Trim();

            lock (_lock)
            {
                var check = _db.SystemAccounts.GetFirst(f => f.Email == email);
                if (check != null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.邮箱已被注册, "已存在该邮箱的账户"));


                var role = _db.Roles.GetById(model.RoleId);
                if (role == null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, "该角色不存在"));

                _db.SystemAccounts.Insert(new SystemAccount()
                {
                    Name = model.Name,
                    Email = email,
                    Password = pwd.ToSHA1(),
                    RoleId = model.RoleId
                });
                return Ok(ApteryxResultApi.Susuccessful());
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "获取指定账户信息",
            OperationId = "Get",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("B", "获取")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<ResultSystemAccountRoleModel>))]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Get([SwaggerParameter("账户ID", Required = false)] long id)
        {
            var account = await _db.SystemAccounts.GetByIdAsync(id);
            if (account == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.账户不存在));
            var role = await _db.Roles.GetByIdAsync(account.RoleId);
            return Ok(ApteryxResultApi.Susuccessful(new ResultSystemAccountRoleModel(account, role)));
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "修改账户与密码",
            OperationId = "Put",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("C", "修改账户与密码", isMustHave: true)]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> EditAccountPwd([FromBody] EditPwdSystemAccountModel model)
        {
            var accountId = HttpContext.GetAccountId();

            var oldpwd = model.OldPassword.ToSHA1();
            var account = await _db.SystemAccounts.GetFirstAsync(f => f.Id == accountId && f.Email == model.OldEmail && f.Password == oldpwd);
            if (account == null)
            {
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.账号或密码错误));
            }

            var check = await _db.SystemAccounts.GetFirstAsync(f => f.Email == model.NewEmail.Trim() && f.Id != account.Id);
            if (check != null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.邮箱已被注册, "已存在该邮箱的账户"));

            var result = account.Clone();

            account.Email = model.NewEmail;
            account.Password = model.NewPassword.ToSHA1();

            //if (!_db.SystemAccounts.AsUpdateable(account).ExecuteCommandHasChange())
            //    return Ok(ApteryxResultApi.Susuccessful("没有任何变更"));

            _db.SystemAccounts.Update(account);

            await _db.Logs.InsertAsync(new Log(accountId, "SystemAccount", ActionMethods.改, "修改账户与密码", account.ToJson(), result.ToJson()));
            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpPost("query")]
        [SwaggerOperation(
            Summary = "查询",
            OperationId = "Query",
            Tags = new[] { "SystemAccount" }
        )]
        [ApiRoleDescription("D", "查询")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<SystemAccount>>))]
        [SwaggerResponse((int)ApteryxCodes.Unauthorized, null, typeof(ApteryxResult))]
        public async Task<IActionResult> PostQuery([FromBody] QuerySystemAccountModel model)
        {
            var page = model.Page;
            var limit = model.Limit;

            var query = _db.SystemAccounts.AsQueryable();

            if (!model.Email.IsNullOrWhiteSpace())
                query = query.Where(w => w.Email.Contains(model.Email));

            if (model.RoleId != null)
                query = query.Where(w => w.RoleId == model.RoleId);

            var count = query.Count();
            var item = query.ToPageList(page,limit);

            return Ok(ApteryxResultApi.Susuccessful(new PageList<SystemAccount>(count, item)));
        }
    }
}
