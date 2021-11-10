using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendResponseProcessor<TRequest, TResponse> : QueueResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public SendResponseProcessor(SendMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
