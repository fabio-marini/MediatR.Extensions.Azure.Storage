using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueRequestBehavior<TRequest> : QueueRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public QueueRequestBehavior(IOptions<QueueMessageOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }

    public class QueueRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public QueueRequestBehavior(IOptions<QueueMessageOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
