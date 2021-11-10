using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class QueueResponseBehaviorBase<TRequest, TResponse> : ResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public QueueResponseBehaviorBase(SendMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public QueueResponseBehaviorBase(ReceiveMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public QueueResponseBehaviorBase(DeleteMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
