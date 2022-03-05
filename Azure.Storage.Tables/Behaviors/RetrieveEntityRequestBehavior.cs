using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class RetrieveEntityRequestBehavior<TRequest> : RetrieveEntityRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public RetrieveEntityRequestBehavior(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }

    public class RetrieveEntityRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public RetrieveEntityRequestBehavior(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
