using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class BlobRequestBehaviorBase<TRequest> : BlobRequestBehaviorBase<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public BlobRequestBehaviorBase(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public BlobRequestBehaviorBase(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public BlobRequestBehaviorBase(DeleteBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }

    public abstract class BlobRequestBehaviorBase<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public BlobRequestBehaviorBase(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public BlobRequestBehaviorBase(DeleteBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public BlobRequestBehaviorBase(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
