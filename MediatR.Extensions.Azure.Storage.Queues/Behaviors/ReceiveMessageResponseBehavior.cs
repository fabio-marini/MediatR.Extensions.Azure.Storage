using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveMessageResponseBehavior<TRequest, TResponse> : QueueResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ReceiveMessageResponseBehavior(ReceiveMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
