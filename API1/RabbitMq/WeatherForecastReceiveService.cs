using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherServiceApi.Models;

namespace WeatherServiceApi.RabbitMq
{
    public class WeatherForecastReceiveService : BasicReceiveService<WeatherForecast>
    {
        public WeatherForecastReceiveService(ILogger<BasicReceiveService<WeatherForecast>> logger, IOptions<RabbitMqOptions> options) : base(logger, options)
        {
        }

        public override Task HandleMessageAsync(WeatherForecast message)
        {
            this._logger.LogInformation("Received weather, TemperateC: " + message.TemperatureC);
            return Task.CompletedTask;
        }
    }
}