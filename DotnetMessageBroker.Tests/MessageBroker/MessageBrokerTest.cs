using DotnetMessageBroker.Offset.Interface;
using DotnetMessageBroker.Tests.Utils.Extensions;

namespace DotnetMessageBroker.MessageBroker.Tests
{
    public class MessageBrokerTests
    {
        [Fact]
        public void Publish_EnqueuesMessageInQueue()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string message = "TestMessage";

            // Act
            messageBroker.Publish(topic, message);
            var messagesResult = messageBroker.GetMessagesInTopic(topic);

            // Assert
            Assert.Single(messagesResult);
        }

        [Fact]
        public void Subscribe_LoadsHistoryForQueue()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");

            // Act
            messageBroker.LoadHistoryForQueue(topic, groupId);

            // Assert
            Assert.Equal(2, messageBroker.GetMessagesInQueue(topic).Count);
        }

        [Fact]
        public void ClearMessagesByGroup_ClearsMessagesForGroup()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");
            messageBroker.Subscribe(topic, groupId);
            var offsetTracker = messageBroker.GetOffsetTracker();

            // Act
            messageBroker.ClearMessagesByGroup(topic, groupId);
            int resultOffSetByGroup = offsetTracker.GetOffset(topic, groupId);


            // Assert
            Assert.Equal(0, resultOffSetByGroup);
        }

        [Fact]
        public void ClearMessages_ClearsAllMessagesForTopic()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");

            // Act
            messageBroker.ClearMessages(topic);

            // Assert
            Assert.Empty(messageBroker.GetMessagesInTopic(topic));
        }

        [Fact]
        public void Unsubscribe_ClearsMessagesForGroupAndRemovesTopicIfNoGroupsLeft()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");
            messageBroker.Subscribe(topic, groupId);
            var offsetTracker = messageBroker.GetOffsetTracker();

            // Act
            messageBroker.Unsubscribe(topic, groupId);

            // Assert
            Assert.Empty(messageBroker.GetMessagesInTopic(topic));
            Assert.False(offsetTracker.SingleGroupInTopic(topic, groupId));
        }
    }

   
}
