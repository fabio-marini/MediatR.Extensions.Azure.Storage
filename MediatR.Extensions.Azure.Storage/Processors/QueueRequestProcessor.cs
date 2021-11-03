using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public QueueRequestProcessor(IOptions<QueueMessageOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
