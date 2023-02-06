using Apteryx.Routing.Role.Authority.RDS;
using Apteryx.Routing.Role.Authority.RDS.Entities.ApteryxConfigs;
using Apteryx.WebApi;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("PostgreSQL");
var db = new SqlSugarScope(new ConnectionConfig
{
    ConnectionString = conn,
    IsAutoCloseConnection = true,
    DbType = DbType.PostgreSQL,
    ConfigureExternalServices = new ConfigureExternalServices
    {
        EntityService = (c, p) =>
        {
            // int?  decimal?这种 isnullable=true
            if (c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                p.IsNullable = true;
            }
        }
    }
});
#if DEBUG
db.Aop.OnLogExecuting = (sql, pars) =>
{
    //Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
    //5.0.8.2 获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
    Console.WriteLine(UtilMethods.GetSqlString(DbType.PostgreSQL, sql, pars));
};
#endif

db.DbMaintenance.CreateDatabase();
db.CodeFirst.InitTables(typeof(WeatherForecast));
builder.Services.AddSingleton<ISqlSugarClient>(db);

builder.Services.AddApteryxAuthority(new ApteryxConfig()
{
    IsSecurityToken = true,
    AESConfig = new AES256Setting()
    {
        Key = "fND+T_yo@wc!$uEEw!mDjqN9wYcvuO2I",
        IV = "70w@Ox_nF*%0G*KE"
    },
    TokenConfig = new TokenSetting()
    {
        Audience = "www.apteryx.com.cn",
        Expires = 7200,
        Key = "102esdjflskjdflkjsf29384023iksdjflk",
        Issuer = "apteryx"
    },
    DbConnection = new DbConnectionConfig()
    {
        ConnectionString = conn,
        DbType = DbType.PostgreSQL
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseApteryxSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
