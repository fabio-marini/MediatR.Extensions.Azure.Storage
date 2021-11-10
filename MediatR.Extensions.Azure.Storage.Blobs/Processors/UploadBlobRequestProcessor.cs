using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobRequestProcessor<TRequest> : BlobRequestProcessorBase<TRequest>
    {
        public UploadBlobRequestProcessor(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
