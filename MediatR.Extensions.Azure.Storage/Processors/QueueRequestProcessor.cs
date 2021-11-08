using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public QueueRequestProcessor(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
