namespace DotnetMessageBroker.MessageBroker.Interface
{
    public interface IMessageBroker
    {
        void LoadHistoryForQueue(string topic, string groupId);
        void ClearMessages(string topic);
        void ClearMessagesByGroup(string topic, string groupId);
    }
}
