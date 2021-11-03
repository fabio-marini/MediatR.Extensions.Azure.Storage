using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadRequestBehavior<TRequest> : UploadRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public UploadRequestBehavior(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }

    public class UploadRequestBehavior<TRequest, TResponse> : RequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public UploadRequestBehavior(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
