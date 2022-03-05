using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadBlobRequestBehavior<TRequest> : DownloadBlobRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public DownloadBlobRequestBehavior(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }

    public class DownloadBlobRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DownloadBlobRequestBehavior(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
}
