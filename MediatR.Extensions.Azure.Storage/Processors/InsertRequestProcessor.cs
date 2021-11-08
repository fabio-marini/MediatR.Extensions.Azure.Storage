using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public InsertRequestProcessor(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
