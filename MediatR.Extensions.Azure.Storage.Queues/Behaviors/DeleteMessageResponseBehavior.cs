using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteMessageResponseBehavior<TRequest, TResponse> : QueueResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteMessageResponseBehavior(DeleteMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
