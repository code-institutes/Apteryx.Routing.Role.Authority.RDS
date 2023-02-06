using apteryx.common.extend.Helpers;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SqlSugar;
using Swashbuckle.AspNetCore.Annotations;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public class ApteryxInitializeDataService
    {
        private readonly ISugarUnitOfWork<ApteryxDbContext> _context;
        private readonly IActionDescriptorCollectionProvider actionDescriptor;
        public ApteryxInitializeDataService(IActionDescriptorCollectionProvider collectionProvider, ISugarUnitOfWork<ApteryxDbContext> context)
        {
            this._context = context;
            this.actionDescriptor = collectionProvider;
        }
        /// <summary>
        /// 初始化账户与角色
        /// </summary>
        public void InitAccountRole()
        {
            //刷新路由
            RefreshRoute();

            //创建账户
            using (var db = _context.CreateContext())
            {
                var act = db.SystemAccounts.AsQueryable();
                if (!act.Any())
                {
                    var role = db.Roles.GetFirst(f => f.Name == "超管" && f.AddType == AddTypes.程序);
                    if (role == null)
                    {
                        role = new Role()
                        {
                            Name = "超管",
                            AddType = AddTypes.程序,
                            Description = "系统默认超级管理员",
                        };
                        db.Roles.Insert(role);

                        db.SystemAccounts.Insert(new SystemAccount()
                        {
                            Email = "wyspaces@outlook.com",
                            Password = "admin1234".ToSHA1(),
                            IsSuper = true,
                            RoleId = role.Id
                        });

                        var routes = db.Routes.GetList();
                        foreach (var route in routes)
                        {
                            db.RoleRoutes.Insert(new RoleRoute()
                            {
                                RoleId = role.Id,
                                RouteId = route.Id
                            });
                        }
                    }
                }
                db.Commit();
            }
        }
        /// <summary>
        /// 刷新路由
        /// </summary>
        public void RefreshRoute()
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

            using (var db = _context.CreateContext())
            {
                foreach (var route in db.Routes.AsQueryable().Where(w => w.AddType == AddTypes.程序).ToList())
                {
                    var validRoute = arrRoutes.FirstOrDefault(a => a.CtrlFullName == route.CtrlFullName && a.Tag == route.Tag);
                    if (validRoute != null)
                    {
                        route.CtrlFullName = validRoute.CtrlFullName;
                        route.Method = validRoute.Method;
                        route.Name = validRoute.Name;
                        route.Description = validRoute.Description;
                        route.Path = validRoute.Path;
                        db.Routes.Update(route);
                        arrRoutes.Remove(validRoute);
                    }
                    else
                    {
                        db.Routes.DeleteById(route.Id);
                        //将路由从所有角色中删除
                        db.RoleRoutes.AsDeleteable().Where(d => d.RouteId == route.Id);
                    }
                }

                if (arrRoutes.Any())
                    db.Routes.InsertRange(arrRoutes);
                db.Commit();
            }
        }
    }
}
