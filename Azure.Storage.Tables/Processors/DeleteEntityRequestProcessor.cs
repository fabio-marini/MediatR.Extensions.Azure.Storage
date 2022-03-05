using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteEntityRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public DeleteEntityRequestProcessor(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
