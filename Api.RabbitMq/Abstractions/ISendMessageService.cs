namespace Api.RabbitMq.Abstractions
{
    public interface ISendMessageService
    {
        void SendMessage<T>(T message);
    }
}