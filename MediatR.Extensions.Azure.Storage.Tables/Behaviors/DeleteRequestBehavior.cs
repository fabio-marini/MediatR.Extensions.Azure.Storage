using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteRequestBehavior<TRequest> : DeleteRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public DeleteRequestBehavior(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }

    public class DeleteRequestBehavior<TRequest, TResponse> : TableRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteRequestBehavior(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
