using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public UploadRequestProcessor(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }
}
