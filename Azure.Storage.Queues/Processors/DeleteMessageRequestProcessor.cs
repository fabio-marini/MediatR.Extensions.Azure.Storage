using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteMessageRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public DeleteMessageRequestProcessor(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
