using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteEntityResponseBehavior<TRequest, TResponse> : TableResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteEntityResponseBehavior(DeleteEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
