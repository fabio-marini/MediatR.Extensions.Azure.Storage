using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteBlobResponseBehavior<TRequest, TResponse> : BlobResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteBlobResponseBehavior(DeleteBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
