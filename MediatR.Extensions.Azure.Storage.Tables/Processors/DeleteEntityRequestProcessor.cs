using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteEntityRequestProcessor<TRequest> : TableRequestProcessorBase<TRequest>
    {
        public DeleteEntityRequestProcessor(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
