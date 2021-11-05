using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class RequestBehaviorBase<TRequest> : RequestBehaviorBase<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public RequestBehaviorBase(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }

        public RequestBehaviorBase(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }

        public RequestBehaviorBase(IOptions<QueueMessageOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null) : base(opt, ctx, log)
        {
        }
    }

    public abstract class RequestBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICommand<TRequest> cmd;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public RequestBehaviorBase(IOptions<InsertEntityOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new InsertEntityCommand<TRequest>(opt, ctx, log);
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestBehaviorBase(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new UploadBlobCommand<TRequest>(opt, ctx, log);
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public RequestBehaviorBase(IOptions<QueueMessageOptions<TRequest>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new QueueMessageCommand<TRequest>(opt, ctx, log);
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
