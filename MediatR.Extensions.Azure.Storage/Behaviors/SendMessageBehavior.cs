using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendMessageBehavior<TRequest> : SendMessageBehavior<TRequest, Unit> where TRequest : IRequest
    {
        public SendMessageBehavior(IOptions<SendMessageOptions<TRequest>> opt, PipelineContext ctx, ILogger log = null)
            : base(opt, ctx, log)
        {
        }
    }

    public class SendMessageBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
    {
        private readonly IOptions<SendMessageOptions<TRequest>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public SendMessageBehavior(IOptions<SendMessageOptions<TRequest>> opt, PipelineContext ctx, ILogger log = null)
        {
            // this parameter is required: if an instance is not supplied, it will be created using the default ctor
            // (which will set IsEnabled = false) - no additional validation is required...
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (opt.Value.IsEnabled == false)
            {
                // behavior is disabled - skip
                log.LogDebug("Behavior {Behavior} is not enabled, invoking next behavior in the chain", this.GetType().Name);

                return await next();
            }

            if (opt.Value.QueueClient == null)
            {
                // no table configured - skip
                log.LogError("Behavior {Behavior} requires a valid QueueClient", this.GetType().Name);

                return await next();
            }

            if (opt.Value.QueueMessage == null)
            {
                // behavior is enabled, but no QueueMessage func specified - use default
                log.LogDebug("Behavior {Behavior} is using the default QueueMessage delegate", this.GetType().Name);

                opt.Value.QueueMessage = (req, ctx) =>
                {
                    var messagePayload = JsonConvert.SerializeObject(req);

                    return BinaryData.FromString(messagePayload);
                };
            }

            try
            {
                var msg = opt.Value.QueueMessage(request, ctx);

                await opt.Value.QueueClient.SendMessageAsync(msg, opt.Value.Visibility, opt.Value.TimeToLive, cancellationToken);

                log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Behavior {Behavior} failed, invoking next behavior in the chain", this.GetType().Name);
            }

            return await next();
        }
    }
}
