namespace DotnetMessageBroker.MessageBroker.Interface
{
    public interface IPublisher<T>
    {
        void Publish(string queueName, T message);
    }
}
