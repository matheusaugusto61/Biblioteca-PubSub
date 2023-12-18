using DotnetMessageBroker.Offset.Interface;
using DotnetMessageBroker.Tests.Utils.Extensions;

namespace DotnetMessageBroker.MessageBroker.Tests
{
    public class MessageBrokerTests
    {
        [Fact]
        public void Publish_When_PublisherMessageInTopic_Should_SaveMessageInHistory()
        {
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string message = "TestMessage";

            messageBroker.Publish(topic, message);
            var messagesResult = messageBroker.GetMessagesInTopic(topic);

            Assert.Single(messagesResult);
        }

        [Fact]
        public void LoadHistoryForQueue_When_ExistsMessagesInHistory_Should_LoadMessagesInQueue()
        {
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");

            messageBroker.LoadHistoryForQueue(topic, groupId);

            Assert.Equal(2, messageBroker.GetMessagesInQueue(topic).Count);
        }

        [Fact]
        public void ClearMessagesByGroup_When_ThereIsAGroupSubscribeToTheTopic_Should_ResetTheOffsetGroup()
        {
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");
            messageBroker.Subscribe(topic, groupId);
            var offsetTracker = messageBroker.GetOffsetTracker();

            messageBroker.ClearMessagesByGroup(topic, groupId);
            int resultOffSetByGroup = offsetTracker.GetOffset(topic, groupId);

            Assert.Equal(0, resultOffSetByGroup);
        }

        [Fact]
        public void ClearMessagesByGroup_When_TopicIsEmpty_Should_ReturnsThrowArgumentNullException()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            var topic = string.Empty;
            var groupId = "group1";

            Assert.Throws<ArgumentNullException>(() => messageBroker.ClearMessagesByGroup(topic, groupId));
        }

        [Fact]
        public void ClearMessagesByGroup_When_GroupIdIsEmpty_Should_ReturnsThrowArgumentNullException()
        {
            // Arrange
            var messageBroker = new MessageBroker<string>();
            var topic = "Topic";
            var groupId = string.Empty;

            Assert.Throws<ArgumentNullException>(() => messageBroker.ClearMessagesByGroup(topic, groupId));
        }

        [Fact]
        public void ClearMessages_When_ThereAreMessagesInTheTopic_Should_DeleteAllMessages()
        {
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");

            messageBroker.ClearMessages(topic);

            Assert.Empty(messageBroker.GetMessagesInTopic(topic));
        }

        [Fact]
        public void Unsubscribe_When_HasValidGroupAndTopic_Should_ClearsMessagesForGroupAndRemovesTopicIfNoGroupsLeft()
        {
            var messageBroker = new MessageBroker<string>();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            messageBroker.Publish(topic, "Message1");
            messageBroker.Publish(topic, "Message2");
            messageBroker.Subscribe(topic, groupId);
            var offsetTracker = messageBroker.GetOffsetTracker();

            messageBroker.Unsubscribe(topic, groupId);

            Assert.Empty(messageBroker.GetMessagesInTopic(topic));
            Assert.False(offsetTracker.SingleGroupInTopic(topic, groupId));
        }
    }

   
}
