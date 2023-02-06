using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SqlSugar;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Apteryx.Routing.Role.Authority.RDS
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApteryxAuthorityServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddApteryxAuthority(this IServiceCollection services, ApteryxConfig config)
        {
            if (config.DbConnection == null)
                throw new Exception("“DbConnection”配置不能为空！");
            if (config.TokenConfig == null)
                throw new Exception("“TokenConfig”配置不能为空！");
            if (config.IsSecurityToken)
                if (config.AESConfig == null)
                    throw new Exception("当开启加密Token设置后，“AESConfig”配置不能为空！");
            services.AddControllers(option =>
            {
                option.Filters.Add<ConsoleAuthorizeAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });

            //注入配置
            services.AddSingleton(config);

            services.AddApteryxSqlsugar(config.DbConnection);

            //注入权限
            services.AddAuthentication()
                .AddJwtBearer(config.AuthenticationScheme, options =>
                {
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Query["access_token"];
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            // Write to the response in any way you wish
                            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                            context.Response.Headers.Append("Access-Control-Allow-Origin", " * ");
                            context.Response.Headers.Append("Cache-Control", "no-cache");
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            context.Response.ContentType = "application/json; charset=utf-8";
                            context.Response.WriteAsJsonAsync(ApteryxResultApi.Fail(ApteryxCodes.Unauthorized));
                            return Task.CompletedTask;
                        }
                        //,
                        //OnAuthenticationFailed = context =>
                        //{
                        //    context.Response.StatusCode = (int)HttpStatusCode.OK;
                        //    context.Response.ContentType = "application/json; charset=utf-8";
                        //    return context.Response.WriteAsJsonAsync(CGIObjectResultApi.Fail(ResponseCode.鉴权失败));
                        //    //return Task.CompletedTask;
                        //}
                    };
                    if (config.IsSecurityToken)
                    {
                        if (config.AESConfig == null)
                            throw new Exception("");
                        options.SecurityTokenValidators.Clear();
                        options.SecurityTokenValidators.Add(new TokenValidator(config.AESConfig.Key, config.AESConfig.IV));
                    }
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, //是否验证Issuer
                        ValidIssuer = config.TokenConfig.Issuer, //Issuer，这两项和前面签发jwt的设置一致

                        ValidateAudience = true, //是否验证Audience
                        ValidAudience = config.TokenConfig.Audience, //Audience

                        ValidateLifetime = true, //是否验证失效时间

                        ValidateIssuerSigningKey = true, //是否验证SecurityKey
                        IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.TokenConfig.Key)), //拿到SecurityKey

                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddSwaggerExamples();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("zh1.0", new OpenApiInfo
                {
                    Title = "Apteryx RestfulAPIs Documentation",
                    Version = "v1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Apteryx Developer",
                        Email = "wyspaces@outlook.com"
                    }
                });
                c.EnableAnnotations();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                c.ExampleFilters();
                c.AddEnumsWithValuesFixFilters();
                c.OperationFilter<SecurityRequirementsOperationFilter>(false);
                // or use the generic method, e.g. c.OperationFilter<SecurityRequirementsOperationFilter<MyCustomAttribute>>();

                // if you're using the SecurityRequirementsOperationFilter, you also need to tell Swashbuckle you're using OAuth2
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "使用承载方案的标准授权标头。 例子: \"Bearer {access token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                //c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Apteryx.Routing.Role.Authority.RDS.xml"));
            });
            services.AddScoped<ApteryxInitializeDataService>();
            return services;
        }
    }
}
