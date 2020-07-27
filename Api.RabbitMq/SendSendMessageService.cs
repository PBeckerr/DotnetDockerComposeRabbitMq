using System.Text;
using Api.RabbitMq.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Api.RabbitMq
{
    public class SendSendMessageService : ISendMessageService
    {
        private readonly IModel _channel;
        private readonly IConnection _conn;
        private readonly ConnectionFactory _factory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ILogger<SendSendMessageService> _logger;

        public SendSendMessageService(ILogger<SendSendMessageService> logger, IOptions<RabbitMqOptions> options)
        {
            this._logger = logger;
            this._factory = new ConnectionFactory {HostName = options.Value.Host, UserName = options.Value.UserName, Password = options.Value.Password};
            this._conn = this._factory.CreateConnection();
            this._channel = this._conn.CreateModel();

            this._jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
        }

        public void SendMessage<T>(T message)
        {
            string json = JsonConvert.SerializeObject(message, this._jsonSerializerSettings);
            var body = Encoding.UTF8.GetBytes(json);
            this.EnsureQueueCreated<T>();
            _channel.BasicPublish(exchange: typeof(T).Name,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            this._logger.LogInformation(" [x] Published {0} to RabbitMQ", json);
        }

        private void EnsureQueueCreated<T>()
        {
            _channel.ExchangeDeclare(exchange: typeof(T).Name, type: ExchangeType.Fanout);
        }
    }
}