using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveMessageRequestBehavior<TRequest> : ReceiveMessageRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public ReceiveMessageRequestBehavior(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }

    public class ReceiveMessageRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ReceiveMessageRequestBehavior(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
