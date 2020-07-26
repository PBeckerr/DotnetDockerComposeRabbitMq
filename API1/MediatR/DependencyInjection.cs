using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WeatherServiceApi.MediatR.Behaviors;

namespace WeatherServiceApi.MediatR
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            // Can be enabled to support slow api
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestSimulatorSlowApiBehavior<,>));

            return services;
        }
    }
}