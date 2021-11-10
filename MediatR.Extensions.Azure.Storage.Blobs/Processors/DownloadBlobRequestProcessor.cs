using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadBlobRequestProcessor<TRequest> : BlobRequestProcessorBase<TRequest>
    {
        public DownloadBlobRequestProcessor(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
