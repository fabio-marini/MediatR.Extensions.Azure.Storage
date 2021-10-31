using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadRequestProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly UploadBlobCommand<TRequest> cmd;
        private readonly ILogger log;

        public UploadRequestProcessor(UploadBlobCommand<TRequest> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(UploadBlobCommand<TRequest>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await cmd.ExecuteAsync(request, cancellationToken);

                log.LogInformation("Processor {Processor} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Processor {Processor} failed, returning", this.GetType().Name);
            }
        }
    }
}
