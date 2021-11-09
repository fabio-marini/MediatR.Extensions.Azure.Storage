using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendMessageOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual QueueClient QueueClient { get; set; }
        public virtual Func<TMessage, PipelineContext, BinaryData> QueueMessage { get; set; }
        public virtual TimeSpan? Visibility { get; set; }
        public virtual TimeSpan? TimeToLive { get; set; }

        // (optional) use the received message to update the message
        public virtual Func<QueueMessage, PipelineContext, TMessage, Task> Select { get; set; }
    }
}
