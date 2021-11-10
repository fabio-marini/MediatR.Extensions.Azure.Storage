using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteBlobResponseProcessor<TRequest, TResponse> : BlobResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteBlobResponseProcessor(DeleteBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
