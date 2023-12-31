﻿namespace DotnetMessageBroker.MessageBroker.Interface
{
    public interface ISubscriber
    {
        void Subscribe(string queue, string groupId);
        void Unsubscribe(string queue, string groupId);
    }
}
