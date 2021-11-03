using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class ResponseProcessorBase<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICommand<TResponse> cmd;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ResponseProcessorBase(IOptions<InsertEntityOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new InsertEntityCommand<TResponse>(opt, ctx, log);
            this.log = log ?? NullLogger.Instance;
            this.ctx = ctx;
        }

        public ResponseProcessorBase(IOptions<QueueMessageOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new QueueMessageCommand<TResponse>(opt, ctx, log);
            this.log = log ?? NullLogger.Instance;
            this.ctx = ctx;
        }

        public ResponseProcessorBase(IOptions<UploadBlobOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new UploadBlobCommand<TResponse>(opt, ctx, log);
            this.log = log ?? NullLogger.Instance;
            this.ctx = ctx;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await cmd.ExecuteAsync(response, cancellationToken);

                log.LogInformation("Processor {Processor} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                if (ctx?.Exceptions != null)
                {
                    ctx.Exceptions.Add(ex);
                }

                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Processor {Processor} failed, returning", this.GetType().Name);
            }
        }
    }
}
