using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadRequestBehavior<TRequest> : DownloadRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public DownloadRequestBehavior(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }

    public class DownloadRequestBehavior<TRequest, TResponse> : BlobRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DownloadRequestBehavior(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
}
