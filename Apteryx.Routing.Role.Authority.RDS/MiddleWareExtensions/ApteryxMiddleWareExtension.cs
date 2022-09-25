using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public static class ApteryxMiddleWareExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApteryxSwaggerUI(this IApplicationBuilder app, string routePrefix = "apteryx")
        {
            app.UseSwagger(c => { c.RouteTemplate = routePrefix + "/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Apteryx RestfulAPIs Documentation";
                c.SwaggerEndpoint($"/{routePrefix}/zh1.0/swagger.json", "Restful APIs Documentation");
                c.RoutePrefix = routePrefix;
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(ModelRendering.Example);
                c.DefaultModelsExpandDepth(1);
                c.DefaultModelExpandDepth(1);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.None);//文档展开形式
                c.EnableDeepLinking();
                c.EnableFilter();
                c.MaxDisplayedTags(20);
                //c.ShowExtensions();
                c.EnableValidator();
            });

            return app;
        }
    }
}
