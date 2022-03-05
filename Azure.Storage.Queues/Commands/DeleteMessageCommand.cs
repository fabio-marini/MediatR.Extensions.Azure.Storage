using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteMessageCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<QueueOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public DeleteMessageCommand(IOptions<QueueOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
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

                return;
            }

            if (opt.Value.QueueClient == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid QueueClient");
            }

            if (opt.Value.Delete == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid Delete delegate");
            }

            try
            {
                var msg = await opt.Value.Delete(ctx, message);

                if (msg == null)
                {
                    log.LogDebug("Command {Command} found no message to delete", this.GetType().Name);

                    return;
                }

                var res = await opt.Value.QueueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, cancellationToken);

                log.LogDebug("Command {Command} completed with status {StatusCode}", this.GetType().Name, res.Status);
            }
            catch (Exception ex)
            {
                log.LogDebug(ex, "Command {Command} failed with message: {Message}", this.GetType().Name, ex.Message);

                throw new CommandException($"Command {this.GetType().Name} failed, see inner exception for details", ex);
            }
        }
    }
}
