using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadRequestBehavior<TRequest> : UploadRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public UploadRequestBehavior(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }

    public class UploadRequestBehavior<TRequest, TResponse> : BlobRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public UploadRequestBehavior(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) 
            : base(cmd, ctx, log)
        {
        }
    }
}
