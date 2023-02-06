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
        private readonly ISugarUnitOfWork<ApteryxDbContext> _context;

        public RoleController(ISugarUnitOfWork<ApteryxDbContext> context)
        {
            _context = context;
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
            using (var db = _context.CreateContext())
            {
                var role = await db.Roles.GetFirstAsync(f => f.Name == model.Name.Trim());
                if (role != null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色已存在, $"角色名：\"{model.Name}\"已存在"));

                role = new Role()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description.Trim()
                };

                await db.Roles.InsertAsync(role);

                var mustRoutes = await db.Routes.GetListAsync(w => w.IsMustHave == true);
                foreach (var route in mustRoutes)
                {
                    db.RoleRoutes.AsInsertable(new RoleRoute()
                    {
                        RoleId = role.Id,
                        RouteId = route.Id
                    }).ExecuteReturnSnowflakeId();
                }

                foreach (var routeId in model.RouteIds)
                {
                    if (db.Routes.AsQueryable().First(a => a.Id == routeId && a.IsMustHave == false) != null)
                        await db.RoleRoutes.InsertAsync(new RoleRoute()
                        {
                            RoleId = role.Id,
                            RouteId = routeId
                        });
                }
                await db.Logs.InsertAsync(new Log(HttpContext.GetAccountId(), "ApteryxRole", ActionMethods.添, "添加角色", null, role.ToJson()));
                db.Commit();

                return Ok(ApteryxResultApi.Susuccessful(role));
            }
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
            using (var db = _context.CreateContext())
            {
                var role = await db.Roles.GetByIdAsync(roleId);
                if (role == null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在));

                if (await db.Roles.GetFirstAsync(a => a.Name == model.Name && a.Id != model.Id) != null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色已存在, $"角色名：\"{model.Name}\"已存在"));

                var result = role.Clone();

                role.Name = model.Name;
                role.Description = model.Description;

                db.RoleRoutes.Delete(w => w.RoleId == roleId);

                var mustRoutes = await db.Routes.GetListAsync(w => w.IsMustHave == true);
                foreach (var route in mustRoutes)
                {
                    db.RoleRoutes.Insert(new RoleRoute()
                    {
                        RoleId = role.Id,
                        RouteId = route.Id
                    });
                }

                foreach (var routeId in model.RouteIds)
                {
                    if (db.Routes.AsQueryable().First(a => a.Id == routeId && a.IsMustHave == false) != null)
                        await db.RoleRoutes.InsertAsync(new RoleRoute()
                        {
                            RoleId = role.Id,
                            RouteId = routeId
                        });
                }

                if (db.Roles.Update(role))
                    await db.Logs.InsertAsync(new Log(HttpContext.GetAccountId(), "ApteryxRole", ActionMethods.改, "编辑角色", result.ToJson(), role.ToJson()));

                db.Commit();
                return Ok(ApteryxResultApi.Susuccessful(role));
            }
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

            using (var db = _context.CreateContext())
            {
                var account = await db.SystemAccounts.GetFirstAsync(f => f.Id == accountId);
                var routes = await db.Routes.GetListAsync();
                var data = new ResultOwnRouteModel()
                {
                    Role = await db.Roles.GetByIdAsync(account.RoleId),
                    GroupRoutes = routes.GroupBy(g => g.CtrlName).Select(s => new GroupRouteModel()
                    {
                        Title = s.Key,
                        RouteInfo = s.Select(ss => new RouteInfoModel()
                        {
                            IsOwn = db.RoleRoutes.GetFirst(f => f.RoleId == account.RoleId && f.RouteId == ss.Id) != null,
                            Route = ss
                        })
                    })
                };
                return Ok(ApteryxResultApi.Susuccessful(data));
            }
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
            using (var db = _context.CreateContext())
            {
                var role = await db.Roles.GetByIdAsync(id);
                if (role == null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, $"角色不存在,ID:{id}"));

                var routes = await db.Routes.GetListAsync();

                var data = new ResultOwnRouteModel()
                {
                    Role = role,
                    GroupRoutes = routes.GroupBy(g => g.CtrlName).Select(s => new GroupRouteModel()
                    {
                        Title = s.Key,
                        RouteInfo = s.Select(ss => new RouteInfoModel()
                        {
                            IsOwn = db.RoleRoutes.GetFirst(f => f.RoleId == role.Id && f.RouteId == ss.Id) != null,
                            Route = ss
                        })
                    })
                };
                return Ok(ApteryxResultApi.Susuccessful(data));
            }
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
            using (var db = _context.CreateContext())
            {
                var role = await db.Roles.GetByIdAsync(id);
                if (role == null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.角色不存在, $"角色不存在,ID:{id}"));

                if (role.AddType == AddTypes.程序)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.系统角色, "系统默认角色禁止删除！"));

                var groupId = SnowFlakeSingle.Instance.NextId();
                var sysAccountId = HttpContext.GetAccountId();

                db.RoleRoutes.Delete(w => w.RoleId == id);
                foreach (var sysAccount in db.SystemAccounts.GetList(w => w.RoleId == id))
                {
                    db.SystemAccounts.DeleteById(sysAccount.Id);
                    await db.Logs.InsertAsync(new Log(sysAccountId, "SystemAccount", ActionMethods.删, "删除角色联动删除账户", sysAccount.ToJson(), null, groupId));
                }

                await db.Roles.DeleteByIdAsync(id);

                await db.Logs.InsertAsync(new Log(sysAccountId, "Role", ActionMethods.删, "删除角色", role.ToJson(), null, groupId));
                db.Commit();

                return Ok(ApteryxResultApi.Susuccessful());
            }
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
            using (var db = _context.CreateContext())
            {
                var result = db.Roles.AsQueryable().ToList().OrderByDescending(d => d.LastTime).ToList();
                return Ok(ApteryxResultApi.Susuccessful(result));
            }
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
