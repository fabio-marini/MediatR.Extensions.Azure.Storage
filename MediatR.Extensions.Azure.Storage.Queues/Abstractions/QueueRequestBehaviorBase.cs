using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class QueueRequestBehaviorBase<TRequest> : QueueRequestBehaviorBase<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public QueueRequestBehaviorBase(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public QueueRequestBehaviorBase(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public QueueRequestBehaviorBase(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }

    public abstract class QueueRequestBehaviorBase<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public QueueRequestBehaviorBase(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public QueueRequestBehaviorBase(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public QueueRequestBehaviorBase(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
