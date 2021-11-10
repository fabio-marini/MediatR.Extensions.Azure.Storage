using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class RetrieveEntityRequestProcessor<TRequest> : TableRequestProcessorBase<TRequest>
    {
        public RetrieveEntityRequestProcessor(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
