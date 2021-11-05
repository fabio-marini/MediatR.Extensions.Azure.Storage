﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class ResponseBehaviorBase<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICommand<TResponse> cmd;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ResponseBehaviorBase(IOptions<InsertEntityOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new InsertEntityCommand<TResponse>(opt, ctx, log);
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public ResponseBehaviorBase(IOptions<UploadBlobOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new UploadBlobCommand<TResponse>(opt, ctx, log);
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public ResponseBehaviorBase(IOptions<QueueMessageOptions<TResponse>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.cmd = new QueueMessageCommand<TResponse>(opt, ctx, log);
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await next();

            try
            {
                await cmd.ExecuteAsync(response, cancellationToken);

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

            return response;
        }
    }
}
