using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public InsertRequestProcessor(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
