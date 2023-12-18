using DotnetMessageBroker.MessageBroker;

var messageBroker = new MessageBroker<string>();

messageBroker.MessageReceived += (sender, args) =>
{
    Console.WriteLine($"Mensagem processada no tópico '{args.Topic}' Grupo {args.GroupId}: {args.Message}");
};

string topic = "exemplo";
string message1 = "Mensagem 1";

messageBroker.Publish(topic, message1);

messageBroker.Subscribe("exemplo", "processEmail");
messageBroker.Subscribe("exemplo", "processWhatsapp");


Console.ReadKey();