namespace DotnetMessageBroker.Offset.Interface
{
    public interface IOffsetTracker
    {
        int GetOffset(string topic, string groupId);
        void SetOffset(string topic, string groupId, int offset=0);
        bool HasOffset(string topic);
        bool HasOffsetByGroup(string topic, string groupId);
        void ClearOffsets(string topic);
        void ClearOffsetsByGroup(string topic, string groupId);
        bool SingleGroupInTopic(string topic, string groupId);
        Dictionary<string, int>.KeyCollection ListGroupsInTopic(string topic);
    }
}
