using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueRequestBehavior<TRequest> : QueueRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public QueueRequestBehavior(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
    public class QueueRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public QueueRequestBehavior(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
