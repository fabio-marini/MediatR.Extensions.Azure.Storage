using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertRequestBehavior<TRequest> : InsertRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public InsertRequestBehavior(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }

    public class InsertRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public InsertRequestBehavior(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
