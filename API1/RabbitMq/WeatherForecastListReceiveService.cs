using System.Threading.Tasks;
using Api.RabbitMq;
using Api.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherServiceApi.Message.Domain;

namespace WeatherServiceApi.RabbitMq
{
    public class WeatherForecastListReceiveService : BasicReceiveService<WeatherForecastList>
    {
        public WeatherForecastListReceiveService(ILogger<BasicReceiveService<WeatherForecastList>> logger, IOptions<RabbitMqOptions> options) : base(logger, options)
        {
        }

        protected override Task HandleMessageAsync(WeatherForecastList message)
        {
            foreach (var weatherForecast in message.Entities)
            {
                this._logger.LogInformation(weatherForecast.ToString());
            }
            return Task.CompletedTask;
        }
    }
}