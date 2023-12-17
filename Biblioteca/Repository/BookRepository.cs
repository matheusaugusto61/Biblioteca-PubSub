using Biblioteca.Model;
using MessagingSystem.Brokers;
using System.Text.Json;

namespace Biblioteca.Repository
{
    public class BookRepository
    {
        private readonly MessageBroker<string> _messageBroker;
        public BookRepository()
        {
            _messageBroker = new MessageBroker<string>("C:\\temp");
        }

        public void SaveBook(Book book)
        {
            _messageBroker.Publish("QueueBook", JsonSerializer.Serialize(book));
        }
    }
}
