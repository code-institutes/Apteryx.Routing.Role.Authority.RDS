using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SqlSugar;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Apteryx.Routing.Role.Authority.RDS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ConsoleAuthorizeAttribute : Attribute, IAuthorizationFilter, IActionFilter
    {
        private readonly ISugarUnitOfWork<ApteryxDbContext> _context;
        public ConsoleAuthorizeAttribute(ISugarUnitOfWork<ApteryxDbContext> context)
        {
            this._context = context;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result == null)
            {
                try
                {
                    context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.发生未知错误, context.Exception.Message))
                    {
                        StatusCode = 200
                    };
                    context.Exception = null;
                }
                catch (Exception e)
                {
                    context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.发生未知错误, e.Message))
                    {
                        StatusCode = 200
                    };
                    context.Exception = null;
                }
                return;
            }
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (!context.ModelState.IsValid)
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };
                context.Result = new OkObjectResult(ApteryxResultApi.Fail(ApteryxCodes.字段验证未通过, JsonSerializer.Serialize(context.ModelState
                    .Where(w => w.Value.Errors.FirstOrDefault() != null)
                    .Select(s => new FieldValid { Field = s.Key, Error = s.Value.Errors.Select(s1 => s1.ErrorMessage) }), options)));
                return;
            }
            return;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var method = context.HttpContext.Request.Method;
                var template = $"/{context.ActionDescriptor.AttributeRouteInfo.Template}";
                var accountId = long.Parse(context.HttpContext.User.Identity.Name);

                using(var db = _context.CreateContext())
                {
                    var systemAccount = db.SystemAccounts.GetById(accountId);
                    if (systemAccount.IsSuper)
                        return;

                    var route = db.Routes.GetFirst(f => f.Method == method && f.Path == template);
                    if (route != null)
                    {
                        //var roleRoute = _db.RoleRoutes.FindOne(f => f.RoleId == systemAccount.RoleId && f.RouteId == route.Id);
                        var roleRoute = db.RoleRoutes.GetFirst(f => f.Id == systemAccount.RoleId && f.RouteId == route.Id);
                        if (roleRoute != null)
                            return;
                    }
                    context.Result = new BadRequestObjectResult(ApteryxResultApi.Fail(ApteryxCodes.权限不足)) { StatusCode = 200 };
                    return;
                }                
            }
            return;
        }
    }
}
