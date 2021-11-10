using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveMessageRequestProcessor<TRequest> : QueueRequestProcessorBase<TRequest>
    {
        public ReceiveMessageRequestProcessor(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
