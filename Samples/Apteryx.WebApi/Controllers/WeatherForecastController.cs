using Apteryx.Routing.Role.Authority.RDS;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Apteryx.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly SqlSugarScope _db;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, ISqlSugarClient sqlSugar)
        {
            _logger = logger;
            _db = sqlSugar as SqlSugarScope;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            //_sqlSugar.Insertable(new SystemAccount()
            //{
            //    Email = "test@test.com",
            //    Password = "test",
            //    IsSuper = true,
            //    Name = "admin",
            //}).ExecuteReturnSnowflakeId();

            var count = _db.Queryable<WeatherForecast>().Count();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}