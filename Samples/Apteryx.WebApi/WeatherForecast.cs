using SqlSugar;

namespace Apteryx.WebApi
{
    public class WeatherForecast
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}