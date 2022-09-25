using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Swashbuckle.AspNetCore.Annotations;

namespace Apteryx.Routing.Role.Authority.RDS.Controllers
{
    [Authorize(AuthenticationSchemes = "apteryx")]
    [SwaggerTag("角色服务")]
    [Route("cgi-bin/apteryx/role")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "zh1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public class RoleController : ControllerBase
    {
        private readonly ApteryxDbContext _db;

        public RoleController(ApteryxDbContext context)
        {
            _db = context;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "添加角色",
            OperationId = "Post",
            Tags = new[] { "Role" }
        )]
        [ApiRoleDescription("A", "添加")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Role>))]
        public async Task<IActionResult> Post([FromBody] AddRoleModel model)
        {
            var role = await _db.Roles.GetFirstAsync(f => f.Name == model.Name.Trim());
            if (role != null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色已存在, role));

            role = new Role()
            {
                Name = model.Name.Trim(),
                Description = model.Description.Trim()
            };

            await _db.Roles.InsertAsync(role);

            var mustRoutes = await _db.Routes.GetListAsync(w => w.IsMustHave == true);
            foreach (var route in mustRoutes)
            {
                _db.RoleRoutes.AsInsertable(new RoleRoute()
                {
                    RoleId = role.Id,
                    RouteId = route.Id
                }).ExecuteReturnSnowflakeId();
            }

            foreach (var routeId in model.RouteIds)
            {
                if (_db.Routes.AsQueryable().First(a => a.Id == routeId && a.IsMustHave == false) != null)
                    await _db.RoleRoutes.InsertAsync(new RoleRoute()
                    {
                        RoleId = role.Id,
                        RouteId = routeId
                    });
            }
            await _db.Logs.InsertAsync(new Log(HttpContext.GetAccountId(), "ApteryxRole", ActionMethods.添, "添加角色", null, role.ToJson()));

            return Ok(ApteryxResultApi.Susuccessful(role));
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "编辑角色",
            OperationId = "Put",
            Tags = new[] { "Role" }
        )]
        [ApiRoleDescription("B", "编辑")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Role>))]
        public async Task<IActionResult> Put([FromBody] EditRoleModel model)
        {
            var roleId = model.Id;
            var role = await _db.Roles.GetByIdAsync(roleId);
            if (role == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在));

            if (await _db.Roles.GetFirstAsync(a => a.Name == model.Name && a.Id != model.Id) != null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色已存在, $"角色名：\"{model.Name}\"已存在"));

            var result = role.Clone();

            role.Name = model.Name;
            role.Description = model.Description;

            _db.RoleRoutes.Delete(w => w.RoleId == roleId);

            var mustRoutes = await _db.Routes.GetListAsync(w => w.IsMustHave == true);
            foreach (var route in mustRoutes)
            {
                _db.RoleRoutes.Insert(new RoleRoute()
                {
                    RoleId = role.Id,
                    RouteId = route.Id
                });
            }

            foreach (var routeId in model.RouteIds)
            {
                if (_db.Routes.AsQueryable().First(a => a.Id == routeId && a.IsMustHave == false) != null)
                    await _db.RoleRoutes.InsertAsync(new RoleRoute()
                    {
                        RoleId = role.Id,
                        RouteId = routeId
                    });
            }

            if (_db.Roles.Update(role))
                await _db.Logs.InsertAsync(new Log(HttpContext.GetAccountId(), "ApteryxRole", ActionMethods.改, "编辑角色", result.ToJson(), role.ToJson()));

            return Ok(ApteryxResultApi.Susuccessful(role));
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "获取账户所有权限",
            OperationId = "Auth",
            Description = "获取当前登录账户的权限",
            Tags = new[] { "Role" }
        )]
        [ApiRoleDescription("C", "获取拥有权限", isMustHave: true)]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<ResultOwnRouteModel>>))]
        public async Task<IActionResult> GetAuth()
        {
            var accountId = HttpContext.GetAccountId();
            var account = await _db.SystemAccounts.GetFirstAsync(f => f.Id == accountId);
            var routes = await _db.Routes.GetListAsync();

            var data = new ResultOwnRouteModel()
            {
                Role = await _db.Roles.GetByIdAsync(account.RoleId),
                GroupRoutes = routes.GroupBy(g => g.CtrlName).Select(s => new GroupRouteModel()
                {
                    Title = s.Key,
                    RouteInfo = s.Select(ss => new RouteInfoModel()
                    {
                        IsOwn = _db.RoleRoutes.GetFirst(f => f.RoleId == account.RoleId && f.RouteId == ss.Id) != null,
                        Route = ss
                    })
                })
            };
            return Ok(ApteryxResultApi.Susuccessful(data));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "获取指定角色的权限",
            OperationId = "Get",
            Tags = new[] { "Role" }
        )]
        [ApiRoleDescription("D", "获取角色权限")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<ResultOwnRouteModel>>))]
        public async Task<IActionResult> Get([SwaggerParameter("角色ID", Required = true)] long id)
        {
            var role = await _db.Roles.GetByIdAsync(id);
            if (role == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, "该角色不存在"));

            var routes = await _db.Routes.GetListAsync();

            var data = new ResultOwnRouteModel()
            {
                Role = role,
                GroupRoutes = routes.GroupBy(g => g.CtrlName).Select(s => new GroupRouteModel()
                {
                    Title = s.Key,
                    RouteInfo = s.Select(ss => new RouteInfoModel()
                    {
                        IsOwn = _db.RoleRoutes.GetFirst(f => f.RoleId == role.Id && f.RouteId == ss.Id) != null,
                        Route = ss
                    })
                })
            };
            return Ok(ApteryxResultApi.Susuccessful(data));
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "删除角色",
            OperationId = "Delete",
            Tags = new[] { "Role" }
        )]
        [ApiRoleDescription("E", "删除")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Delete([SwaggerParameter("角色ID", Required = true)] long id)
        {
            var role = await _db.Roles.GetByIdAsync(id);
            if (role == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, "该角色不存在"));

            var groupId = SnowFlakeSingle.Instance.NextId();
            var sysAccountId = HttpContext.GetAccountId();

            _db.RoleRoutes.Delete(w => w.RoleId == id);
            foreach (var sysAccount in _db.SystemAccounts.GetList(w => w.RoleId == id))
            {
                _db.SystemAccounts.DeleteById(sysAccount.Id);
                await _db.Logs.InsertAsync(new Log(sysAccountId, "SystemAccount", ActionMethods.删, "删除角色联动删除账户", sysAccount.ToJson(), null, groupId));
            }
            
            await _db.Roles.DeleteByIdAsync(id);

            await _db.Logs.InsertAsync(new Log(sysAccountId, "Role", ActionMethods.删, "删除角色", role.ToJson(), null, groupId));

            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpPost("query")]
        [SwaggerOperation(
            Summary = "查询",
            OperationId = "Query",
            Tags = new[] { "Role" }
        )]
        [ApiRoleDescription("F", "查询")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<Role>>))]
        public async Task<IActionResult> PostQuery()
        {
            var result = _db.Roles.AsQueryable().ToList().OrderByDescending(d => d.LastTime).ToList();
            return Ok(ApteryxResultApi.Susuccessful(result));
        }

        //[HttpGet("report/usage/{roleId}")]
        //[SwaggerOperation(
        //    Summary = "使用率",
        //    Description = "统计指定角色所有账户的使用率",
        //    OperationId = "Item",
        //    Tags = new[] { "Role" }
        //)]
        //[ApiRoleDescription("G", "统计使用率")]
        //[SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<ReportUsageRoleModel>))]
        //public async Task<IActionResult> Usage(string roleId)
        //{
        //    var role = await _db.Roles.FindOneAsync(f => f.Id == roleId);
        //    if (role == null)
        //        return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在));

        //    var accounts = _db.SystemAccounts.Where(w => w.RoleId == role.Id);

        //    var item = accounts.Select(s =>
        //    {
        //        var usage = new ReportUsageRoleModel()
        //        {
        //            Account = s,
        //            LogNum = _db.Logs.CountDocuments(log => log.SystemAccountId == s.Id)
        //        };
        //        usage.Account.Password = "";
        //        return usage;
        //    }).OrderByDescending(d => d.LogNum);
        //    return Ok(ApteryxResultApi.Susuccessful(item.ToList()));
        //}
    }
}
