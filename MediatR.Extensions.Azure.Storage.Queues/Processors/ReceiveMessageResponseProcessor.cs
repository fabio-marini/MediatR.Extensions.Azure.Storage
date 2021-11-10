using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveMessageResponseProcessor<TRequest, TResponse> : QueueResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ReceiveMessageResponseProcessor(ReceiveMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
