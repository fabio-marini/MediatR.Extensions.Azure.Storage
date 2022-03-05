using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteEntityRequestBehavior<TRequest> : DeleteEntityRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public DeleteEntityRequestBehavior(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }

    public class DeleteEntityRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteEntityRequestBehavior(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
