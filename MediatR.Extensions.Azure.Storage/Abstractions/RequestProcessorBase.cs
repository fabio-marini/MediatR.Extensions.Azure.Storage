using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class RequestProcessorBase<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ICommand<TRequest> cmd;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        #region Table Constructors

        public RequestProcessorBase(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        #endregion

        #region Blob Constructors

        public RequestProcessorBase(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(DeleteBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(DownloadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        #endregion

        #region Queue Constructors

        public RequestProcessorBase(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(ReceiveMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestProcessorBase(DeleteMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        #endregion

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await cmd.ExecuteAsync(request, cancellationToken);

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
