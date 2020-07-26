using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherServiceApi.MediatR.Commands;
using WeatherServiceApi.MediatR.Queries;
using WeatherServiceApi.Models;
using WeatherServiceApi.RabbitMq;

namespace WeatherServiceApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISendMessageService _sendMessageService;

        public WeatherForecastController(IMediator mediator, ISendMessageService sendMessageService)
        {
            this._mediator = mediator;
            this._sendMessageService = sendMessageService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get([FromQuery] GetAllWeatherForecastsQuery query)
        {
            var forecasts = await this._mediator.Send(query)
                                      .ConfigureAwait(false);

            foreach (var weatherForecast in forecasts)
            {
                this._sendMessageService.SendMessage(weatherForecast);
            }

            return forecasts;
        }

        [HttpPost]
        public async Task<WeatherForecast> Post(CreateWeathterForecastCommand command) =>
            await this._mediator.Send(command)
                      .ConfigureAwait(false);
    }
}