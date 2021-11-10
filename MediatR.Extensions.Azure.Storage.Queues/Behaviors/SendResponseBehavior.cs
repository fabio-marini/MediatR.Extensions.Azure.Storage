using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendResponseBehavior<TRequest, TResponse> : QueueResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public SendResponseBehavior(SendMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
