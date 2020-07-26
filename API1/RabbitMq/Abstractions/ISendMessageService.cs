namespace WeatherServiceApi.RabbitMq
{
    public interface ISendMessageService
    {
        void SendMessage<T>(T message);
    }
}