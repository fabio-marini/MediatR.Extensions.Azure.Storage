using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteBlobRequestBehavior<TRequest> : DeleteBlobRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public DeleteBlobRequestBehavior(DeleteBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }

    public class DeleteBlobRequestBehavior<TRequest, TResponse> : BlobRequestBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteBlobRequestBehavior(DeleteBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
            : base(cmd, ctx, log)
        {
        }
    }
}
