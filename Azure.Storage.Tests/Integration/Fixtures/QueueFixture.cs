using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public class QueueFixture
    {
        public QueueFixture()
        {
            QueueClient = new QueueClient("UseDevelopmentStorage=true", "integration-tests");
            QueueClient.CreateIfNotExists();

            Messages = new Queue<QueueMessage>();
        }

        public QueueClient QueueClient { get; }

        public Queue<QueueMessage> Messages { get; }

        public void GivenQueueIsEmpty() => QueueClient.ClearMessages();

        public void ThenQueueIsEmpty()
        {
            var allMessages = QueueClient.PeekMessages(16);

            allMessages.Value.Any().Should().BeFalse();
        }

        public void ThenQueueHasMessages(int messageCount)
        {
            var allMessages = QueueClient.PeekMessages(16);

            allMessages.Value.Should().HaveCount(messageCount);
        }
    }
}
