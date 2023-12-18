using DotnetMessageBroker.Event;
using DotnetMessageBroker.MessageBroker.Interface;
using DotnetMessageBroker.Offset.Interface;
using DotnetMessageBroker.Offset;
using System.Text.RegularExpressions;

namespace DotnetMessageBroker.MessageBroker;

/// <summary>
///     Classe que implementa um sistema de publicação e assinatura de mensagens genéricas em tópicos específicos.
/// </summary>
/// <typeparam name="T">Tipo de mensagem a ser publicada e assinada.</typeparam>
public class MessageBroker<T> : IMessageBroker, IPublisher<T>, ISubscriber
{
    private readonly HashSet<string> _interestedTopics;
    private readonly Dictionary<string, List<T>> _allMessages;
    private readonly Dictionary<string, Queue<T>> _queueMessages;
    private readonly IOffsetTracker _offsetTracker;

    public event EventHandler<MessageReceivedEventArgs<T>>? MessageReceived;

    public MessageBroker()
    {
        _interestedTopics = new HashSet<string>();
        _allMessages = new Dictionary<string, List<T>>();
        _queueMessages = new Dictionary<string, Queue<T>>();
        _offsetTracker = new OffsetTracker();
    }

    /// <summary>
    ///     Publica uma mensagem em um tópico específico.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Publish(string topic, T message)
    {
        CheckIfTopicIsValid(topic);
        CheckIfMessageIsValid(message);

        EnsureTopicExists(topic);

        _queueMessages[topic].Enqueue(message);

        NotifyAllSubscribers(topic);
    }

    /// <summary>
    ///     Inscreve um grupo em um tópico específico, permitindo o recebimento de mensagens disponíveis no tópico.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Subscribe(string topic, string groupId)
    {
        CheckIfTopicAndGroupIsValid(topic, groupId);

        _interestedTopics.Add(topic);
        EnsureOffsetExists(topic, groupId);

        LoadHistoryForQueue(topic, groupId);

        NotifySubscribers(topic, groupId);
    }

    /// <summary>
    ///     Carrega o histórico de mensagens disponíveis para um grupo que acabou de se inscrever em um tópico.
    /// </summary>
    public void LoadHistoryForQueue(string topic, string groupId)
    {
        EnsureTopicExists(topic);
        LoadMessagesIntoQueue(topic, groupId);
    }

    /// <summary>
    ///     Limpa o controle de index de onde determinado grupo estava na leitura das mensagens da fila naquele tópico 
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void ClearMessagesByGroup(string topic, string groupId)
    {
        CheckIfTopicAndGroupIsValid(topic, groupId);

        _offsetTracker.ClearOffsetsByGroup(topic, groupId);
    }

    /// <summary>
    ///     Limpa todas as mensagens que já foram executadas por determinada fila em determinado tópico.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void ClearMessages(string topic)
    {
        CheckIfTopicIsValid(topic);

        if (_allMessages.ContainsKey(topic))
        {
            _allMessages[topic].Clear();
            _queueMessages[topic].Clear();
            _offsetTracker.ClearOffsets(topic);
        }
    }

    /// <summary>
    ///     Limpa mensagens de um grupo e apaga o topico caso não tenha mais grupos nele.
    /// </summary>
    public void Unsubscribe(string topic, string groupId)
    {
        if (_offsetTracker.SingleGroupInTopic(topic, groupId))
            _allMessages.Remove(topic);

        ClearMessagesByGroup(topic, groupId);
    }

    /// <summary>
    ///     Garante que a fila e o historico de mensagens de um determinado tópico exista.
    /// </summary>
    private void EnsureTopicExists(string topic)
    {
        if (!_allMessages.ContainsKey(topic))
            _allMessages[topic] = new List<T>();

        if (!_queueMessages.ContainsKey(topic))
            _queueMessages[topic] = new Queue<T>();
    }

    /// <summary>
    ///     Garante que o offset de um grupo para determinado tópico exista.
    /// </summary>
    private void EnsureOffsetExists(string topic, string groupId)
    {
        if (!_offsetTracker.HasOffsetByGroup(topic, groupId))
            _offsetTracker.SetOffset(topic, groupId);
    }

    /// <summary>
    ///     Faz a iteração das mensagens de uma fila para determinado grupo a partir do offset do mesmo.
    /// </summary>
    private void LoadMessagesIntoQueue(string topic, string groupId)
    {
        int offset = _offsetTracker.GetOffset(topic, groupId);

        for (int i = offset; i < _allMessages[topic].Count; i++)
        {
            _queueMessages[topic].Enqueue(_allMessages[topic][i]);
            _offsetTracker.SetOffset(topic, groupId, ++offset);
        }
    }

    /// <summary>
    ///     Notifica os inscritos de um determinado tópico e grupo que existem mensagens a serem processadas.
    /// </summary>
    private void NotifySubscribers(string topic, string groupId)
    {
        if (!_interestedTopics.Contains(topic))
        {
            var message = _queueMessages[topic].Dequeue();
            _allMessages[topic].Add(message);
            return;
        }

        int offset = _offsetTracker.GetOffset(topic, groupId);

        while (_queueMessages[topic].Count > 0)
        {
            var message = _queueMessages[topic].Dequeue();
            _offsetTracker.SetOffset(topic, groupId, ++offset);
            OnMessageReceived(topic, groupId, message);
        }
    }

    /// <summary>
    ///     Notifica todos os grupos inscritos em um determinado tópico sobre a existência de uma mensagem a ser processada.
    /// </summary>
    private void NotifyAllSubscribers(string topic)
    {
        var message = _queueMessages[topic].Dequeue();

        if (!_interestedTopics.Contains(topic))
        {
            _allMessages[topic].Add(message);
            return;
        }

        _allMessages[topic].Add(message);

        foreach (var groupId in _offsetTracker.GetAllGroupsInTopic(topic))
        {
            int offset = _offsetTracker.GetOffset(topic, groupId);
            OnMessageReceived(topic, groupId, message);
            _offsetTracker.SetOffset(topic, groupId, ++offset);

        }
    }

    private void CheckIfTopicIsValid(string topic)
    {
        if (!topic.Any())
            throw new ArgumentNullException(nameof(topic), MessageBrokerConstants.TopicCannotBeEmpty);
    }

    private void CheckIfTopicAndGroupIsValid(string topic, string groupId)
    {
        if (!topic.Any())
            throw new ArgumentNullException(nameof(topic), MessageBrokerConstants.TopicCannotBeEmpty);
        if (!groupId.Any())
            throw new ArgumentNullException(nameof(groupId), MessageBrokerConstants.GroupCannotBeEmpty);
    }

    private void CheckIfMessageIsValid(T message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message), MessageBrokerConstants.MessageCannotBeNull);
    }

    /// <summary>
    ///     Dispara um evento notificando os inscritos que uma mensagem foi recebida em um determinado tópico e grupo.
    /// </summary>
    protected virtual void OnMessageReceived(string topic, string groupId, T message)
    {
        MessageReceived?.Invoke(this, new MessageReceivedEventArgs<T>(topic, groupId, message));
    }

}
