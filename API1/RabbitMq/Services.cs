using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderServiceApi.Core;
using OrderServiceApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderServiceApi.RabbitMq
{
    public interface IMessageService
    {
        void SendMessage<T>(T message);
    }

    public class MessageService : IMessageService
    {
        private readonly IModel _channel;
        private readonly IConnection _conn;
        private readonly ConnectionFactory _factory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ILogger<MessageService> _logger;
        private readonly Dictionary<Type, List<Action<object>>> onReceiveActions = new Dictionary<Type, List<Action<object>>>();

        public MessageService(ILogger<MessageService> logger)
        {
            this._logger = logger;
            this._factory = new ConnectionFactory {HostName = "rabbitmq", Port = 5672};
            this._factory.UserName = "user";
            this._factory.Password = "bitnami";
            this._conn = this._factory.CreateConnection();
            this._channel = this._conn.CreateModel();
            this._channel.QueueDeclare(
                "hello",
                false,
                false,
                false,
                null
            );

            this._jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
        }

        public void SendMessage<T>(T message)
        {
            string json = JsonConvert.SerializeObject(message, Formatting.None, this._jsonSerializerSettings);
            var body = Encoding.UTF8.GetBytes(json);
            this._channel.BasicPublish(
                "",
                "hello",
                null,
                body
            );
            this._logger.LogInformation(" [x] Published {0} to RabbitMQ", json);
        }
    }

    public abstract class BasicReceiveService<T> : IHostedService
    {
        private IModel _channel;
        private IConnection _conn;
        private ConnectionFactory _factory;
        private JsonSerializerSettings _jsonSerializerSettings;
        protected readonly ILogger<BasicReceiveService<T>> _logger;
        private readonly Dictionary<Type, List<Action<object>>> onReceiveActions = new Dictionary<Type, List<Action<object>>>();

        public BasicReceiveService(ILogger<BasicReceiveService<T>> logger)
        {
            this._logger = logger;
            this._jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
        }

        public abstract Task HandleMessage(T message);
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._factory = new ConnectionFactory {HostName = "rabbitmq", Port = 5672};
            this._factory.UserName = "user";
            this._factory.Password = "bitnami";
            this._conn = this._factory.CreateConnection();
            this._channel = this._conn.CreateModel();
            this._channel.QueueDeclare(
                "hello",
                false,
                false,
                false,
                null
            );

            var consumer = new EventingBasicConsumer(this._channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var deserialized = JsonConvert.DeserializeObject(message, this._jsonSerializerSettings);
                this.HandleMessage((T) deserialized);
            };
            this._channel.BasicConsume(
                "hello",
                true,
                consumer
            );
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._channel.Dispose();
            this._conn.Dispose();
            return Task.CompletedTask;
        }
    }
    
    public class WeatherForecastReceiveService : BasicReceiveService<WeatherForecast>
    {
        public WeatherForecastReceiveService(ILogger<BasicReceiveService<WeatherForecast>> logger) : base(logger)
        {
        }

        public override Task HandleMessage(WeatherForecast message)
        {
            this._logger.LogInformation("Weather: " + message.TemperatureF);
            return Task.CompletedTask;
        }
    }

    public static class DependencyExtensions
    {
        public static IServiceCollection AddMessageServices(this IServiceCollection services)
        {
            MethodInfo methodInfo =
                typeof(ServiceCollectionHostedServiceExtensions)
                    .GetMethods()
                    .FirstOrDefault(p => p.Name == nameof(ServiceCollectionHostedServiceExtensions.AddHostedService));

            if (methodInfo == null)
                throw new Exception($"Impossible to find the extension method '{nameof(ServiceCollectionHostedServiceExtensions.AddHostedService)}' of '{nameof(IServiceCollection)}'.");

            IEnumerable<Type> hostedServices_FromAssemblies = Assembly.GetExecutingAssembly().GetTypes().Where(x => TypeHelper.IsSubclassOfRawGeneric(typeof(BasicReceiveService<>), x) && !x.ContainsGenericParameters);

            foreach (Type hostedService in hostedServices_FromAssemblies) 
            {
                if (typeof(IHostedService).IsAssignableFrom(hostedService))
                {
                    var genericMethod_AddHostedService = methodInfo.MakeGenericMethod(hostedService);
                    _ = genericMethod_AddHostedService.Invoke(obj: null, parameters: new object[] { services }); // this is like calling services.AddHostedService<T>(), but with dynamic T (= backgroundService).
                }
            }

            return services;
        }
    }
}