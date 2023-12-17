using DotnetMessageBroker.Offset.Interface;

namespace DotnetMessageBroker.Offset
{
    public class OffsetTracker : IOffsetTracker
    {
        public Dictionary<string, Dictionary<string, int>> Offsets { get; }

        public OffsetTracker()
        {
            Offsets = new Dictionary<string, Dictionary<string, int>>();
        }

        public int GetOffset(string topic, string groupId)
        {
            if (Offsets.ContainsKey(topic) && Offsets[topic].ContainsKey(groupId))
            {
                return Offsets[topic][groupId];
            }
            return 0;
        }

        /// <summary>
        ///     Método para setar 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="groupId"></param>
        /// <param name="offset"></param>
        public void SetOffset(string topic, string groupId, int offset = 0)
        {
            if (!Offsets.ContainsKey(topic))
            {
                Offsets[topic] = new Dictionary<string, int>();
            }

            Offsets[topic][groupId] = offset;
        }

        /// <summary>
        ///     Método para verificar se existe algum offset para aquele tópico informado
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public bool HasOffset(string topic)
        {
            if (Offsets.ContainsKey(topic))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Método para verificar se existe um offset para aquele grupo e topico informados
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HasOffsetByGroup(string topic, string groupId)
        {
            if (HasOffset(topic) && Offsets[topic].ContainsKey(groupId))
            {
                return true;
            }

            return false;
        }

        public void ClearOffsets(string topic)
        {
            if (HasOffset(topic))
                Offsets.Remove(topic);
        }

        public void ClearOffsetsByGroup(string topic, string groupId)
        {
            if (HasOffsetByGroup(topic, groupId))
                Offsets[topic].Remove(groupId);
        }

        public bool SingleGroupInTopic(string topic, string groupId)
        {
            return HasOffsetByGroup(topic, groupId) && Offsets[topic].Count == 1;
        }

        public Dictionary<string, int>.KeyCollection ListGroupsInTopic(string topic)
        {
            return Offsets[topic].Keys;
        }
    }
}
