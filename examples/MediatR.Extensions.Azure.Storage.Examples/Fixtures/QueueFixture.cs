using Azure.Storage.Queues;
using FluentAssertions;
using Polly;
using System;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public class QueueFixture
    {
        private readonly QueueClient que;
        private readonly ITestOutputHelper log;

        public QueueFixture(QueueClient que, ITestOutputHelper log)
        {
            this.que = que;
            this.log = log;
        }

        public void GivenQueueIsEmpty() => que.ClearMessages();

        public void ThenQueueHasMessages(int expectedCount)
        {
            var retryPolicy = Policy
                .HandleResult<int>(res => res != expectedCount)
                .WaitAndRetry(5, i => TimeSpan.FromSeconds(1));

            var actualCount = retryPolicy.Execute(() =>
            {
                var res = que.PeekMessages(32).Value.Length;

                log.WriteLine($"Queue {que.Name} has {res} messages");

                return res;
            });

            actualCount.Should().Be(expectedCount);
        }

        public void ThenQueueIsEmpty() => ThenQueueHasMessages(0);
    }
}
