using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendRequestBehavior<TRequest> : SendRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public SendRequestBehavior(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
    public class SendRequestBehavior<TRequest, TResponse> : QueueRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public SendRequestBehavior(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
}
