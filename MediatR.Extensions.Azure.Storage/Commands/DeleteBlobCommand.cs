using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteBlobCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<UploadBlobOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public DeleteBlobCommand(IOptions<UploadBlobOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
