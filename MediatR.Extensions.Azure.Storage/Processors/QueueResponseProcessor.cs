using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueResponseProcessor<TRequest, TResponse> : ResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public QueueResponseProcessor(SendMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
