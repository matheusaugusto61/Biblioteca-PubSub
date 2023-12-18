using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetMessageBroker.MessageBroker
{
    public static class MessageBrokerConstants
    {
        public static string TopicCannotBeEmpty = "O valor do tópico não pode ser vazio";
        public static string GroupCannotBeEmpty = "O valor do grupo não pode ser vazio";
        public static string MessageCannotBeNull = "O valor da mensagem não pode ser nula";
    }
}
