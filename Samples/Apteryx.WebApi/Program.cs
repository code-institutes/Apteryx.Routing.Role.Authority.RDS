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
            // int?  decimal?���� isnullable=true
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
    //Console.WriteLine(sql);//���sql,�鿴ִ��sql ������Ӱ��
    //5.0.8.2 ��ȡ�޲�����SQL ��������Ӱ�죬�ر���SQL������ģ�����ʹ��
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
