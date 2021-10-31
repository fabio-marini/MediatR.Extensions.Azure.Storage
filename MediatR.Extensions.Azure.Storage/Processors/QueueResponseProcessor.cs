using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueResponseProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly QueueMessageCommand<TResponse> cmd;
        private readonly ILogger log;

        public QueueResponseProcessor(QueueMessageCommand<TResponse> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(QueueMessageCommand<TResponse>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            await cmd.ExecuteAsync(response, cancellationToken);
        }
    }
}
