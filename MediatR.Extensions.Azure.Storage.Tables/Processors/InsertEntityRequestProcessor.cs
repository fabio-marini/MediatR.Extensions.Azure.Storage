using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityRequestProcessor<TRequest> : TableRequestProcessorBase<TRequest>
    {
        public InsertEntityRequestProcessor(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
