using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadResponseProcessor<TRequest, TResponse> : ResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public UploadResponseProcessor(IOptions<UploadBlobOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
