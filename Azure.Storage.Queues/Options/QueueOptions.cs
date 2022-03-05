using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using MediatR.Extensions.Abstractions;
using System;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set; }

        public virtual QueueClient QueueClient { get; set; }
        public virtual Func<TMessage, PipelineContext, BinaryData> QueueMessage { get; set; }
        public virtual TimeSpan? Visibility { get; set; }
        public virtual TimeSpan? TimeToLive { get; set; }

        // the event that is raised after the message is received (allows using the queue message to modify TMessage)
        public virtual Func<QueueMessage, PipelineContext, TMessage, Task> Received { get; set; }

        // the event that is raised before the message is deleted (allows retrieving the message to be deleted)
        public virtual Func<PipelineContext, TMessage, Task<QueueMessage>> Delete { get; set; }
    }
}
