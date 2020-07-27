using System.Threading.Tasks;
using Api.RabbitMq;
using Api.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherServiceApi.Message.Domain;

namespace WeatherServiceApi.RabbitMq
{
    public class WeatherForecastReceiveService : BasicReceiveService<WeatherForecast>
    {
        public WeatherForecastReceiveService(ILogger<BasicReceiveService<WeatherForecast>> logger, IOptions<RabbitMqOptions> options) : base(logger, options)
        {
        }

        protected override Task HandleMessageAsync(WeatherForecast message)
        {
            this._logger.LogInformation("Received weather, TemperateC: " + message.TemperatureC);
            return Task.CompletedTask;
        }
    }
}