using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteMessageResponseProcessor<TRequest, TResponse> : QueueResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteMessageResponseProcessor(DeleteMessageCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
