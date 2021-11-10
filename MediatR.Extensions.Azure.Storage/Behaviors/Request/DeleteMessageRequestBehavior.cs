using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteMessageRequestBehavior<TRequest> : DeleteMessageRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public DeleteMessageRequestBehavior(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }

    public class DeleteMessageRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteMessageRequestBehavior(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
}
