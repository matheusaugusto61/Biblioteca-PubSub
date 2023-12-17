using DotnetMessageBroker.MessageBroker;
using DotnetMessageBroker.Offset.Interface;

namespace DotnetMessageBroker.Tests.Utils.Extensions
{
    public static class MessageBrokerExtensions
    {
        public static List<T> GetMessagesInTopic<T>(this MessageBroker<T> messageBroker, string topic)
        {
            var allMessages = messageBroker.GetType().GetField("_allMessages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(messageBroker) as Dictionary<string, List<T>>;
            if (allMessages.ContainsKey(topic))
            {
                return allMessages[topic];
            }
            return new List<T>();
        }

        public static Queue<T> GetMessagesInQueue<T>(this MessageBroker<T> messageBroker, string topic)
        {
            var queueMessages = messageBroker.GetType().GetField("_queueMessages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(messageBroker) as Dictionary<string, Queue<T>>;
            if (queueMessages.ContainsKey(topic))
            {
                return queueMessages[topic];
            }
            return new Queue<T>();
        }

        public static IOffsetTracker GetOffsetTracker<T>(this MessageBroker<T> messageBroker)
        {
            return messageBroker.GetType().GetField("_offsetTracker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(messageBroker) as IOffsetTracker;
        }
    }
}
