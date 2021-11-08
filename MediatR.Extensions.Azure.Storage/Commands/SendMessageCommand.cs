using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class SendMessageCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<SendMessageOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public SendMessageCommand(IOptions<SendMessageOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public virtual async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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

            if (msg == null)
            {
                // the queue client supports null messages - don't throw, but log a warning...
                log.LogWarning($"The QueueMessage delegate of Command {this.GetType().Name} returned a null message");
            }

            await opt.Value.QueueClient.SendMessageAsync(msg, opt.Value.Visibility, opt.Value.TimeToLive, cancellationToken);
        }
    }
}
