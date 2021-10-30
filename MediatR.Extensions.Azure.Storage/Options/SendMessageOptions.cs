using Azure.Storage.Queues;
using System;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendMessageOptions<TRequest> : SendMessageOptions<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    public class SendMessageOptions<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual QueueClient QueueClient { get; set; }
        public virtual Func<TRequest, PipelineContext, BinaryData> QueueMessage { get; set; }
        public virtual TimeSpan? Visibility { get; set; }
        public virtual TimeSpan? TimeToLive { get; set; }
    }
}
