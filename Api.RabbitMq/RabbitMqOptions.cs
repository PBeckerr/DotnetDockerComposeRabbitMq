namespace Api.RabbitMq
{
    public class RabbitMqOptions
    {
        public const string RabbitMq = "Rabbitmq";

        public string UserName { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
    }
}