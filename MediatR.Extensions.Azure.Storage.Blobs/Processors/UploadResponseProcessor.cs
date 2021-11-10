using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadResponseProcessor<TRequest, TResponse> : BlobResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public UploadResponseProcessor(UploadBlobCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
