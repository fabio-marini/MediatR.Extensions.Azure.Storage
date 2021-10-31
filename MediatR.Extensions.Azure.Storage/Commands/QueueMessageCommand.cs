using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class QueueMessageCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<QueueMessageOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public QueueMessageCommand(IOptions<QueueMessageOptions<TMessage>> opt, PipelineContext ctx, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            if (opt.Value.IsEnabled == false)
            {
                log.LogDebug("Command {Command} is not enabled, returning", this.GetType().Name);

                return ;
            }

            if (opt.Value.QueueClient == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid QueueClient");
            }

            if (opt.Value.QueueMessage == null)
            {
                log.LogDebug("Command {Command} is using the default QueueMessage delegate", this.GetType().Name);

                opt.Value.QueueMessage = (req, ctx) =>
                {
                    var messagePayload = JsonConvert.SerializeObject(req);

                    return BinaryData.FromString(messagePayload);
                };
            }

            var msg = opt.Value.QueueMessage(message, ctx);

            await opt.Value.QueueClient.SendMessageAsync(msg, opt.Value.Visibility, opt.Value.TimeToLive, cancellationToken);
        }
    }
}
