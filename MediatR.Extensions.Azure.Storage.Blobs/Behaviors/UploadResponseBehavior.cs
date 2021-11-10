using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadResponseBehavior<TRequest, TResponse> : BlobResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public UploadResponseBehavior(UploadBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
