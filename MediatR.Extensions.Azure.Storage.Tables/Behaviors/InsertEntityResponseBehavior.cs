using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityResponseBehavior<TRequest, TResponse> : TableResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public InsertEntityResponseBehavior(InsertEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
