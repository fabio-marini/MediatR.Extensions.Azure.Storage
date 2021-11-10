using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class RetrieveRequestBehavior<TRequest> : RetrieveRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public RetrieveRequestBehavior(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }

    public class RetrieveRequestBehavior<TRequest, TResponse> : TableRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public RetrieveRequestBehavior(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
