namespace DotnetMessageBroker.MessageBroker.Interface
{
    public interface ISubscriber
    {
        void Subscribe(string queue, string groupId, bool readAllMessages);
        void Unsubscribe(string queue, string groupId);
    }
}
