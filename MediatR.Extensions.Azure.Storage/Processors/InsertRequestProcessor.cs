using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertRequestProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly InsertEntityCommand<TRequest> cmd;
        private readonly ILogger log;

        public InsertRequestProcessor(InsertEntityCommand<TRequest> cmd, ILogger log = null)
        {
            this.cmd = cmd ?? throw new ArgumentException($"A valid {nameof(InsertEntityCommand<TRequest>)} is required");
            this.log = log ?? NullLogger.Instance;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            await cmd.ExecuteAsync(request, cancellationToken);
        }
    }
}
