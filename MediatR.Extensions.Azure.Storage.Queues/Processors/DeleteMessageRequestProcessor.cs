using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteMessageRequestProcessor<TRequest> : QueueRequestProcessorBase<TRequest>
    {
        public DeleteMessageRequestProcessor(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
