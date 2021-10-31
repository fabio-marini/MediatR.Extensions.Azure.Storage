using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobBehavior<TRequest> : UploadBlobBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public UploadBlobBehavior(UploadMessageCommand<TRequest> cmd, ILogger log = null) : base(cmd, log)
        {
        }
    }

    public class UploadBlobBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly UploadMessageCommand<TRequest> cmd;
        private readonly ILogger log;

        public UploadBlobBehavior(UploadMessageCommand<TRequest> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(UploadMessageCommand<TRequest>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await cmd.ExecuteAsync(request, cancellationToken);

            return await next();
        }
    }
}
