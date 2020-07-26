using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherServiceApi.Core;

namespace WeatherServiceApi.RabbitMq
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddMessageServices(this IServiceCollection services)
        {
            var methodInfo =
                typeof(ServiceCollectionHostedServiceExtensions)
                    .GetMethods()
                    .FirstOrDefault(p => p.Name == nameof(ServiceCollectionHostedServiceExtensions.AddHostedService));

            if (methodInfo == null)
            {
                throw new Exception(
                    $"Impossible to find the extension method '{nameof(ServiceCollectionHostedServiceExtensions.AddHostedService)}' of '{nameof(IServiceCollection)}'."
                );
            }

            var hostedServices_FromAssemblies = Assembly.GetExecutingAssembly()
                                                        .GetTypes()
                                                        .Where(
                                                            x => TypeHelper.IsSubclassOfRawGeneric(typeof(BasicReceiveService<>), x)
                                                                 && !x.ContainsGenericParameters
                                                        );

            foreach (var hostedService in hostedServices_FromAssemblies)
            {
                if (typeof(IHostedService).IsAssignableFrom(hostedService))
                {
                    var genericMethod_AddHostedService = methodInfo.MakeGenericMethod(hostedService);
                    _ = genericMethod_AddHostedService.Invoke(
                        null,
                        new object[] {services}
                    );
                }
            }

            return services;
        }
    }
}