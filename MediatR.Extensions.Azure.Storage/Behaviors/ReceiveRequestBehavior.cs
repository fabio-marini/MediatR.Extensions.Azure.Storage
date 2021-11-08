using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveRequestBehavior<TRequest> : ReceiveRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public ReceiveRequestBehavior(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
    public class ReceiveRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ReceiveRequestBehavior(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
