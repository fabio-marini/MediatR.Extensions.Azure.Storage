using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityBehavior<TRequest> : InsertEntityBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public InsertEntityBehavior(InsertMessageCommand<TRequest> cmd, ILogger log = null) : base(cmd, log)
        {
        }
    }

    public class InsertEntityBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly InsertMessageCommand<TRequest> cmd;
        private readonly ILogger log;

        public InsertEntityBehavior(InsertMessageCommand<TRequest> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(InsertMessageCommand<TRequest>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await cmd.ExecuteAsync(request, cancellationToken);

            return await next();
        }
    }
}
