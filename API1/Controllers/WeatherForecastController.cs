using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderServiceApi.MediatR.Commands;
using OrderServiceApi.MediatR.Queries;
using OrderServiceApi.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrderServiceApi.RabbitMq;

namespace OrderServiceApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMessageService _messageService;

        public WeatherForecastController(IMediator mediator, IMessageService messageService)
        {
            this._mediator = mediator;
            this._messageService = messageService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get([FromQuery] GetAllWeatherForecastsQuery query)
        {
            var forecasts = await this._mediator.Send(query)
                                      .ConfigureAwait(false);

            foreach (var weatherForecast in forecasts)
            {
                this._messageService.SendMessage(weatherForecast);
            }

            return forecasts;
        }

        [HttpPost]
        public async Task<WeatherForecast> Post(CreateWeathterForecastCommand command)
        {
            return await this._mediator.Send(command)
                             .ConfigureAwait(false);
        }
    }
}