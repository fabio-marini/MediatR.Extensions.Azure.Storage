using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobResponseBehavior<TRequest, TResponse> : BlobResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public UploadBlobResponseBehavior(UploadBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
