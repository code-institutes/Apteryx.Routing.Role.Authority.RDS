using apteryx.common.extend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using Swashbuckle.AspNetCore.Annotations;

namespace Apteryx.Routing.Role.Authority.RDS.Controllers
{
    [Authorize(AuthenticationSchemes = "apteryx")]
    [SwaggerTag("日志服务")]
    [Route("cgi-bin/apteryx/log")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "zh1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public class LogController : ControllerBase
    {
        private readonly ISugarUnitOfWork<ApteryxDbContext> _context;

        public LogController(IConfiguration config, ISugarUnitOfWork<ApteryxDbContext> context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "获取指定日志信息",
            OperationId = "Get",
            Tags = new[] { "Log" }
        )]
        [ApiRoleDescription("A", "获取")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Log>))]
        public async Task<IActionResult> Get([SwaggerParameter("日志ID", Required = true)] long id)
        {
            using (var _db = _context.CreateContext())
            {
                return Ok(ApteryxResultApi.Susuccessful(await _db.Logs.GetByIdAsync(id)));
            }
        }

        [HttpPost("query")]
        [SwaggerOperation(
            Summary = "查询",
            OperationId = "Query",
            Tags = new[] { "Log" }
        )]
        [ApiRoleDescription("B", "查询")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<LogExtModel>>))]
        public async Task<IActionResult> PostQuery([FromBody] QueryLogModel model)
        {
            var page = model.Page;
            var limit = model.Limit;
            var method = model.Method;
            var accountId = model.AccountId;
            var groupId = model.GroupId;
            var key = model.Key;

            using (var _db = _context.CreateContext())
            {
                var query = _db.Logs.AsQueryable();
                if (method != null)
                    query = query.Where(w => w.ActionMethod == method);

                if (accountId != null)
                    query = query.Where(w => w.SystemAccountId == accountId);

                if (groupId != null)
                    query = query.Where(w => w.GroupId == groupId);

                if (!key.IsNullOrWhiteSpace())
                    query = query.Where(w => w.ActionName.Contains(key) || w.Source.Contains(key) || w.After.Contains(key));

                var count = query.Count();
                var data = query.OrderByDescending(o => o.Id).ToPageList(page, limit);
                var listLog = data.ToList().Select(s =>
                {
                    var sysAccount = _db.SystemAccounts.GetById(s.SystemAccountId);
                    var role = _db.Roles.GetById(sysAccount.RoleId);
                    return new LogExtModel()
                    {
                        Id = s.Id,
                        AccountInfo = new ResultSystemAccountRoleModel(sysAccount, role),
                        ActionMethod = s.ActionMethod,
                        ActionName = s.ActionName,
                        TableName = s.MongoCollectionName,
                        CreateTime = s.CreateTime
                    };
                });

                return Ok(ApteryxResultApi.Susuccessful(new PageList<LogExtModel>(count, listLog)));
            }
        }
    }
}