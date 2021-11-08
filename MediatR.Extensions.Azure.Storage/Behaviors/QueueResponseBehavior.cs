using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueResponseBehavior<TRequest, TResponse> : ResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public QueueResponseBehavior(SendMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
