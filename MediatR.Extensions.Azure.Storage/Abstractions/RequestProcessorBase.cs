using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class RequestProcessorBase<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ICommand<TRequest> cmd;
        private readonly ILogger log;

        public RequestProcessorBase(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd ?? new InsertEntityCommand<TRequest>(opt, ctx, log);
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd ?? new UploadBlobCommand<TRequest>(opt, ctx, log);
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(IOptions<QueueMessageOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd ?? new QueueMessageCommand<TRequest>(opt, ctx, log);
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
