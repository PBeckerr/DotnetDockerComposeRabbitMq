using FluentValidation;
using WeatherServiceApi.MediatR.Commands;

namespace WeatherServiceApi.Validation
{
    public class CreateWeathterForecastCommandValidator : AbstractValidator<CreateWeathterForecastCommand>
    {
        public CreateWeathterForecastCommandValidator()
        {
            this.RuleFor(forecast => forecast.TemperatureC)
                .NotEmpty();
            this.RuleFor(forecast => forecast.Date)
                .NotEmpty();
        }
    }
}