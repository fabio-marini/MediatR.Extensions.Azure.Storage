using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class RequestBehaviorBase<TRequest> : RequestBehaviorBase<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public RequestBehaviorBase(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public RequestBehaviorBase(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public RequestBehaviorBase(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }

    public abstract class RequestBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICommand<TRequest> cmd;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public RequestBehaviorBase(InsertEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestBehaviorBase(RetrieveEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestBehaviorBase(DeleteEntityCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestBehaviorBase(UploadBlobCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestBehaviorBase(SendMessageCommand<TRequest> cmd, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = cmd;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await cmd.ExecuteAsync(request, cancellationToken);

                log.LogInformation("Behavior {Behavior} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                if (ctx?.Exceptions != null)
                {
                    ctx.Exceptions.Add(ex);
                }

                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Behavior {Behavior} failed, returning", this.GetType().Name);
            }

            return await next();
        }
    }
}
