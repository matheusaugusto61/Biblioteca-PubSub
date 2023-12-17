using DotnetMessageBroker.Offset;
using DotnetMessageBroker.Offset.Interface;
using System.Collections.Generic;
using Xunit;

namespace DotnetMessageBroker.Offset.Tests
{
    public class OffsetTrackerTests
    {
        [Fact]
        public void GetOffset_When_TopicAndGroupNotSet_Should_ReturnsZero()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();

            int offset = offsetTracker.GetOffset("NonExistingTopic", "NonExistingGroup");

            Assert.Equal(0, offset);
        }

        [Fact]
        public void SetOffset_When_SetsOffsetForTopicAndGroup_ReturnsGetOffsetEqualSetOffset()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();
            string topic = "TestTopic";
            string groupId = "TestGroup";
            int expectedOffset = 10;

            offsetTracker.SetOffset(topic, groupId, expectedOffset);

            int offset = offsetTracker.GetOffset(topic, groupId);
            Assert.Equal(expectedOffset, offset);
        }

        [Fact]
        public void HasOffset_When_SearchingForNonExistentTopicOffset_Should_ReturnsFalse()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();

            bool hasOffset = offsetTracker.HasOffset("NonExistingTopic");

            Assert.False(hasOffset);
        }

        [Fact]
        public void HasOffset_When_SearchingForNonExistentTopicAndGroupOffset_Should_ReturnsFalse()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();

            bool hasOffsetByGroup = offsetTracker.HasOffsetByGroup("NonExistingTopic", "NonExistingGroup");

            Assert.False(hasOffsetByGroup);
        }

        [Fact]
        public void ClearOffsets_When_RemovesOffsetsByTopicAndSearchForTheSame_Should_ReturnsOffsetNonExistent()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();
            string topic = "TestTopic";
            offsetTracker.SetOffset(topic, "Group1", 5);
            offsetTracker.SetOffset(topic, "Group2", 10);

            offsetTracker.ClearOffsets(topic);

            Assert.False(offsetTracker.HasOffset(topic));
        }

        [Fact]
        public void ClearOffsetsByGroup_When_RemovesOffsetForGroupInTopic_Should_ReturnsOffsetForGroupNonExistent()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();
            string topic = "TestTopic";
            string groupId = "Group1";
            offsetTracker.SetOffset(topic, groupId, 5);

            offsetTracker.ClearOffsetsByGroup(topic, groupId);

            Assert.False(offsetTracker.HasOffsetByGroup(topic, groupId));
        }

        //todo revisar teste
        [Fact]
        public void SingleGroupInTopic_ReturnsTrue_WhenSingleGroupInTopic()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();
            string topic = "TestTopic";
            string groupId = "Group1";
            offsetTracker.SetOffset(topic, groupId, 5);

            bool isSingleGroup = offsetTracker.SingleGroupInTopic(topic, groupId);

            Assert.True(isSingleGroup);
        }

        [Fact]
        public void ListGroupsInTopic_When_SearchGroupsInTopic_Should_ReturnsKeyCollection()
        {
            IOffsetTracker offsetTracker = new OffsetTracker();
            string topic = "TestTopic";
            offsetTracker.SetOffset(topic, "Group1", 5);
            offsetTracker.SetOffset(topic, "Group2", 10);

            var groups = offsetTracker.ListGroupsInTopic(topic);

            Assert.Equal(2, groups.Count);
            Assert.Contains("Group1", groups);
            Assert.Contains("Group2", groups);
        }
    }
}
