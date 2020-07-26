using FluentValidation;
using OrderServiceApi.MediatR.Commands;

namespace OrderServiceApi.Validation
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