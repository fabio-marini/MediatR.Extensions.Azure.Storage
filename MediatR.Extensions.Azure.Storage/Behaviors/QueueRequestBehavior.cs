using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueRequestBehavior<TRequest> : QueueRequestBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public QueueRequestBehavior(QueueMessageCommand<TRequest> cmd, ILogger log = null) : base(cmd, log)
        {
        }
    }

    public class QueueRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly QueueMessageCommand<TRequest> cmd;
        private readonly ILogger log;

        public QueueRequestBehavior(QueueMessageCommand<TRequest> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(QueueMessageCommand<TRequest>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                await cmd.ExecuteAsync(request, cancellationToken);

                log.LogInformation("Behavior {Behavior} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Behavior {Behavior} failed, returning", this.GetType().Name);
            }

            return await next();
        }
    }
}
