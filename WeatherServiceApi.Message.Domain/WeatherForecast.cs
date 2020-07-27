using System;
using System.Collections.Generic;

namespace WeatherServiceApi.Message.Domain
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int) (this.TemperatureC / 0.5556);

        public override string ToString() => $"{nameof(this.Date)}: {this.Date}, {nameof(this.TemperatureC)}: {this.TemperatureC}, {nameof(this.TemperatureF)}: {this.TemperatureF}";
    }

    public class WeatherForecastList
    {
        public List<WeatherForecast> Entities { get; set; }
    }
}