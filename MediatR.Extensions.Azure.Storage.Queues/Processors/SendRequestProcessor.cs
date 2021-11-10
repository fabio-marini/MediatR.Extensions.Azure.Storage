using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendRequestProcessor<TRequest> : QueueRequestProcessorBase<TRequest>
    {
        public SendRequestProcessor(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
