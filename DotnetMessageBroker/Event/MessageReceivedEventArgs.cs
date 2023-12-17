namespace DotnetMessageBroker.Event
{
    public class MessageReceivedEventArgs<T> : EventArgs
    {
        public string Topic { get; }
        public string GroupId { get; }
        public T Message { get; }

        public MessageReceivedEventArgs(string topic, string groupId, T message)
        {
            Topic = topic;
            Message = message;
            GroupId = groupId;
        }
    }
}
