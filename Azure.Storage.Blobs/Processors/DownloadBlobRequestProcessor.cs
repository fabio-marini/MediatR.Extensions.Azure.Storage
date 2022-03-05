using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadBlobRequestProcessor<TRequest> : RequestProcessorBase<TRequest>
    {
        public DownloadBlobRequestProcessor(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
