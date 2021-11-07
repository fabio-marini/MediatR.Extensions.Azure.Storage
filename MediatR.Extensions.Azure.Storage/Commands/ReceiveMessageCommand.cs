using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveMessageCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<QueueMessageOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ReceiveMessageCommand(IOptions<QueueMessageOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public virtual async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
        }
    }
}
