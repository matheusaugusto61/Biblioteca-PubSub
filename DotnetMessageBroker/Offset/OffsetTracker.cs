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

        /// <summary>
        ///     Retorna Offset do grupo em um determinado tópico.
        /// </summary>
        public int GetOffset(string topic, string groupId)
        {
            if (Offsets.ContainsKey(topic) && Offsets[topic].ContainsKey(groupId))
            {
                return Offsets[topic][groupId];
            }
            return 0;
        }

        /// <summary>
        ///     Define o offset do grupo em um determinado tópico.
        /// </summary>
        public void SetOffset(string topic, string groupId, int offset = 0)
        {
            if (!Offsets.ContainsKey(topic))
            {
                Offsets[topic] = new Dictionary<string, int>();
            }

            Offsets[topic][groupId] = offset;
        }

        /// <summary>
        ///     Verifica se existe algum offset para aquele tópico informado.
        /// </summary>
        public bool HasOffset(string topic)
        {
            if (Offsets.ContainsKey(topic))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Verifica se existe um offset para aquele grupo e topico informados.
        /// </summary>
        public bool HasOffsetByGroup(string topic, string groupId)
        {
            if (HasOffset(topic) && Offsets[topic].ContainsKey(groupId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Apaga todos os offsets que existem para determinado tópico.
        /// </summary>
        public void ClearOffsets(string topic)
        {
            if (HasOffset(topic))
                Offsets.Remove(topic);
        }

        /// <summary>
        ///     Reinicia o offset de um grupo para um determinado tópico.
        /// </summary>
        public void ClearOffsetsByGroup(string topic, string groupId)
        {
            if (HasOffsetByGroup(topic, groupId))
                Offsets[topic].Remove(groupId);
        }

        /// <summary>
        ///     Retorna positivo caso o grupo informado seja o unico que possui offset em um determinado tópico.
        /// </summary>
        public bool SingleGroupInTopic(string topic, string groupId)
        {
            return HasOffsetByGroup(topic, groupId) && Offsets[topic].Count == 1;
        }

        /// <summary>
        ///     Retorna todos os grupos com seus offsets que existem para aquele tópico.
        /// </summary>
        public Dictionary<string, int>.KeyCollection GetAllGroupsInTopic(string topic)
        {
            return Offsets[topic].Keys;
        }
    }
}
