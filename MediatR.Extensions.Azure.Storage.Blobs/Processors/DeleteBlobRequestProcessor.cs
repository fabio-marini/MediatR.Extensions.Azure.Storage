using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteBlobRequestProcessor<TRequest> : BlobRequestProcessorBase<TRequest>
    {
        public DeleteBlobRequestProcessor(DeleteBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
