using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityRequestBehavior<TRequest> : InsertEntityRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public InsertEntityRequestBehavior(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }

    public class InsertEntityRequestBehavior<TRequest, TResponse> : TableRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public InsertEntityRequestBehavior(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
