using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadBlobResponseProcessor<TRequest, TResponse> : BlobResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DownloadBlobResponseProcessor(DownloadBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
