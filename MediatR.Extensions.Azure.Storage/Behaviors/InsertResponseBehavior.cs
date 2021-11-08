using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertResponseBehavior<TRequest, TResponse> : ResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public InsertResponseBehavior(InsertEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
