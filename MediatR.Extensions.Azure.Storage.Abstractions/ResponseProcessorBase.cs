using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage.Abstractions
{
    public abstract class ResponseProcessorBase<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICommand<TResponse> cmd;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ResponseProcessorBase(ICommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
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
