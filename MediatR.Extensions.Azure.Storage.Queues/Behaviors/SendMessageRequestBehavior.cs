using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendMessageRequestBehavior<TRequest> : SendMessageRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public SendMessageRequestBehavior(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }

    public class SendMessageRequestBehavior<TRequest, TResponse> : QueueRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public SendMessageRequestBehavior(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
}
