using DotnetMessageBroker.MessageBroker;

var pubSub = new MessageBroker<string>();

pubSub.MessageReceived += (sender, args) =>
{
    Console.WriteLine($"Mensagem recebida no tópico '{args.Topic}' Group {args.GroupId}: {args.Message}");
};

pubSub.Subscribe("topicA", "group1");
pubSub.Subscribe("topicB", "group1");

pubSub.Publish("topicA", $"Mensagem para o tópico A");


pubSub.Publish("topicB", $"Mensagem para o tópico B");

pubSub.Subscribe("topicA", "group1");

pubSub.Publish("topicA", $"Mensagem para o tópico A");

pubSub.Subscribe("topicA", "group1");

pubSub.Subscribe("topicA", "group2");
pubSub.Subscribe("topicA", "group3");

pubSub.Subscribe("topicA", "group2");

pubSub.Subscribe("topicB", "group2");
pubSub.Subscribe("topicB", "group2");

pubSub.Publish("topicA", $"Mensagem para o tópico A especial");

Console.ReadKey();


