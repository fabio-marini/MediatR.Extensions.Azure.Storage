using Azure.Storage.Queues;
using System;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueMessageOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual QueueClient QueueClient { get; set; }
        public virtual Func<TMessage, PipelineContext, BinaryData> QueueMessage { get; set; }
        public virtual TimeSpan? Visibility { get; set; }
        public virtual TimeSpan? TimeToLive { get; set; }
    }
}
