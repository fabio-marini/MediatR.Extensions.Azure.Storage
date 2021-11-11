using Azure.Storage.Queues;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public class QueueFixture
    {
        public QueueFixture()
        {
            QueueClient = new QueueClient("UseDevelopmentStorage=true", "integration-tests");
            QueueClient.CreateIfNotExists();
        }

        public QueueClient QueueClient { get; }
    }
}
