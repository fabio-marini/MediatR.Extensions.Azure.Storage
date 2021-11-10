using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class RetrieveEntityResponseBehavior<TRequest, TResponse> : TableResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public RetrieveEntityResponseBehavior(RetrieveEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
