﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using apteryx.common.extend.Helpers;

namespace Apteryx.Routing.Role.Authority.RDS.Controllers
{
    [Authorize(AuthenticationSchemes = "apteryx")]
    [SwaggerTag("路由服务")]
    [Route("cgi-bin/apteryx/route")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "zh1.0")]
    [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
    public class RouteController : ControllerBase
    {
        private readonly ApteryxDbContext _db;
        private readonly IActionDescriptorCollectionProvider actionDescriptor;

        public RouteController(IActionDescriptorCollectionProvider collectionProvider, ApteryxDbContext context)
        {
            _db = context;
            this.actionDescriptor = collectionProvider;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "添加路由",
            OperationId = "Post",
            Tags = new[] { "Route" }
        )]
        [ApiRoleDescription("A", "添加")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Post([FromBody] AddRouteModel model)
        {
            var path = model.Path.Trim();
            var method = model.Method.Trim();
            var action = await _db.Routes.GetFirstAsync(f => f.Path == path && f.Method == method);
            if (action != null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由已存在));

            await _db.Routes.InsertAsync(new Route()
            {
                CtrlName = model.CtrlName.Trim(),
                Description = model.Description.Trim(),
                Method = method,
                Path = path
            });

            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "编辑路由",
            OperationId = "Put",
            Tags = new[] { "Route" }
        )]
        [ApiRoleDescription("B", "编辑")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Put([FromBody] EditRouteModel model)
        {
            var routeId = model.Id;
            var path = model.Path.Trim();
            var method = model.Method.Trim();

            var route = await _db.Routes.GetByIdAsync(routeId);
            if (route == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由不存在));

            if (route.AddType != AddTypes.人工)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由无权修改, "只能编辑手动添加的路由"));

            var check = await _db.Routes.GetFirstAsync(f => f.Path == path && f.Method == method && f.Id  != routeId);
            if (check != null)
                    return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由已存在, "已存在相同路由数据"));

            var result = route.Clone();

            route.CtrlName = model.CtrlName.Trim();
            route.Description = model.Description.Trim();
            route.Method = method;
            route.Path = path;
            route.LastTime = DateTime.Now;

            if (await _db.Routes.UpdateAsync(route))
                await _db.Logs.InsertAsync(new Log(HttpContext.GetAccountId(), "Route", ActionMethods.改, "编辑路由", result.ToJson(), route.ToJson()));

            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "获取指定路由",
            OperationId = "Get",
            Tags = new[] { "Route" }
        )]
        [ApiRoleDescription("C", "获取")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<Route>))]
        public async Task<IActionResult> Get([SwaggerParameter("路由ID", Required = true)] long id)
        {
            return Ok(ApteryxResultApi.Susuccessful(await _db.Routes.GetByIdAsync(id)));
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "删除路由",
            OperationId = "Delete",
            Tags = new[] { "Route" }
        )]
        [ApiRoleDescription("D", "删除")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult))]
        public async Task<IActionResult> Delete([SwaggerParameter("路由ID", Required = true)] long id)
        {
            var route = await _db.Routes.GetByIdAsync(id);
            if (route == null)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由不存在));

            var sysAccountId = HttpContext.GetAccountId();

            if (route.AddType != AddTypes.人工)
                return Ok(ApteryxResultApi.Fail(ApteryxCodes.路由无权删除, "只能删除手动添加的路由"));

            ////将路由从所有角色中删除
            //await _db.Roles.UpdateManyAsync(u => u.RouteIds.Contains(id), Builders<Role>.Update.Pull(p => p.RouteIds, id));

            //删除角色路由权限
            _db.RoleRoutes.Delete(w => w.RouteId == route.Id);

            //删除路由
            await _db.Routes.DeleteByIdAsync(id);
            //记录日志
            await _db.Logs.InsertAsync(new Log(sysAccountId, "Route", ActionMethods.删, "删除路由", route.ToJson()));

            return Ok(ApteryxResultApi.Susuccessful());
        }

        [HttpPost("query")]
        [SwaggerOperation(
            Summary = "查询",
            OperationId = "Query",
            Tags = new[] { "Route" }
        )]
        [ApiRoleDescription("E", "查询")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<PageList<Route>>))]
        public async Task<IActionResult> PostQuery([FromBody] QueryRouteModel model)
        {
            var title = model.CtrlName;
            var method = model.Method;
            var path = model.Path;

            var query = _db.Routes.AsQueryable();
            if (!model.IsShowMustHave)
                query = query.Where(w => w.IsMustHave == false);

            if (!title.IsNullOrWhiteSpace())
                query = query.Where(w => w.CtrlName.Contains(title));

            if (!method.IsNullOrWhiteSpace())
                query = query.Where(w => w.Method.Contains(method));

            if (!path.IsNullOrWhiteSpace())
                query = query.Where(w => w.Path.Contains(path));

            var totalCount = query.Count();

            var data = query.OrderByDescending(o => o.Id).ToPageList(model.Page, model.Limit);

            return Ok(ApteryxResultApi.Susuccessful(new PageList<Route>(totalCount, data)));
        }


#if DEBUG
        [AllowAnonymous]
#endif
        [HttpGet("refresh")]
        [SwaggerOperation(
            Summary = "刷新",
            OperationId = "Refresh",
            Tags = new[] { "Route" }
        )]
        [ApiRoleDescription("F", "刷新")]
        [SwaggerResponse((int)ApteryxCodes.请求成功, null, typeof(ApteryxResult<IEnumerable<ResultGroupRouteModel>>))]
        public async Task<ActionResult<ApteryxResult<List<Route>>>> GetRefresh()
        {
            List<Route> arrRoutes = new List<Route>();
            foreach (var action in actionDescriptor.ActionDescriptors.Items)
            {

                var ctrlFullName = string.Empty;
                var groupName = string.Empty;
                var actFullName = string.Empty;
                var httpMethods = "GET,POST,PUT,DELETE,PATCH";

                var ctrlActDesc = (ControllerActionDescriptor)action;

                if (action.ActionConstraints.Any())
                {
                    var methodActionConstraint = action.ActionConstraints.First(f => f.GetType() == typeof(HttpMethodActionConstraint)) as HttpMethodActionConstraint;
                    httpMethods = string.Join(',', methodActionConstraint.HttpMethods.Select(s => s));
                }

                ctrlFullName = ctrlActDesc.ControllerTypeInfo.FullName;
                actFullName = action.DisplayName;

                var ctrlSwaggerTagAttr = action.EndpointMetadata.FirstOrDefault(f => f.GetType() == typeof(SwaggerTagAttribute));
                if (ctrlSwaggerTagAttr != null)
                    groupName = ((SwaggerTagAttribute)ctrlSwaggerTagAttr).Description;
                else
                    groupName = ctrlActDesc.ControllerName;

                var apiRoleDescObject = ctrlActDesc.EndpointMetadata.FirstOrDefault(f => f.GetType() == typeof(ApiRoleDescriptionAttribute));

                if (apiRoleDescObject != null)
                {
                    var apiRoleDesc = (ApiRoleDescriptionAttribute)apiRoleDescObject;

                    arrRoutes.Add(new Route()
                    {
                        CtrlFullName = ctrlFullName,
                        CtrlName = groupName,
                        Path = $"/{ctrlActDesc.AttributeRouteInfo.Template}",
                        Method = httpMethods,
                        AddType = AddTypes.程序,
                        Tag = apiRoleDesc.Tag,
                        Name = apiRoleDesc.Name,
                        Description = apiRoleDesc.Description,
                        IsMustHave = apiRoleDesc.IsMustHave
                    });
                }
            }

            foreach (var route in _db.Routes.AsQueryable().Where(w => w.AddType == AddTypes.程序).ToList())
            {
                var validRoute = arrRoutes.FirstOrDefault(a => a.CtrlFullName == route.CtrlFullName && a.Tag == route.Tag);
                if (validRoute != null)
                {
                    route.CtrlFullName = validRoute.CtrlFullName;
                    route.Method = validRoute.Method;
                    route.Name = validRoute.Name;
                    route.Description = validRoute.Description;
                    route.Path = validRoute.Path;
                    await _db.Routes.UpdateAsync(route);
                    arrRoutes.Remove(validRoute);
                }
                else
                {
                    await _db.Routes.DeleteByIdAsync(route.Id);
                    //将路由从所有角色中删除
                    _db.RoleRoutes.AsDeleteable().Where(d => d.RouteId == route.Id);
                }
            }

            if (arrRoutes.Any())
                await _db.Routes.InsertRangeAsync(arrRoutes);

            var item = _db.Routes.GetList().GroupBy(g => g.CtrlName).Select(s => new ResultGroupRouteModel()
            {
                CtrlName = s.Key,
                Routes = s.Select(ss => ss)
            });

            return Ok(ApteryxResultApi.Susuccessful(item));
        }
    }
}
