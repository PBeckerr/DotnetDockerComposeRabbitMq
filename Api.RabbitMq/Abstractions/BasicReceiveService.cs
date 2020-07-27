using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api.RabbitMq.Abstractions
{
    public abstract class BasicReceiveService<T> : IHostedService
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        protected readonly ILogger<BasicReceiveService<T>> _logger;
        private readonly IOptions<RabbitMqOptions> _options;
        private IModel _channel;
        private IConnection _conn;
        private ConnectionFactory _factory;

        protected BasicReceiveService(ILogger<BasicReceiveService<T>> logger, IOptions<RabbitMqOptions> options)
        {
            this._logger = logger;
            this._options = options;
            this._jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._factory = new ConnectionFactory {HostName = this._options.Value.Host, UserName = this._options.Value.UserName, Password = this._options.Value.Password};
            this._factory.UserName = "user";
            this._factory.Password = "bitnami";
            this._conn = this._factory.CreateConnection();
            this._channel = this._conn.CreateModel();
            this._channel.QueueDeclare(
                typeof(T).Name,
                false,
                false,
                false,
                null
            );

            var consumer = new EventingBasicConsumer(this._channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var deserialized = JsonConvert.DeserializeObject(message, this._jsonSerializerSettings);
                await this.HandleMessageAsync((T) deserialized);
            };
            this._channel.BasicConsume(
                typeof(T).Name,
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

        protected abstract Task HandleMessageAsync(T message);
    }
}