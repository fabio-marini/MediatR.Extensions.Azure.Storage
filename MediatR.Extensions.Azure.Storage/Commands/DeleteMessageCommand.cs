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
        private readonly IOptions<QueueMessageOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public DeleteMessageCommand(IOptions<QueueMessageOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public virtual async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

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

            // FIXME: how to delete the correct message? Test with multiple receives in the same pipeline...
            //        use a Queue<QueueMessage> instead of a List<QueueMessage>

            // Send takes BinaryData, returns SendReceipt (has PopReceipt required to delete, see below)
            // Receive takes nothing, returns QueueMessage
            // Delete takes messageId and PopReceipt

            // SendReceipt
            //     This value is required to delete the Message. If deletion fails using this popreceipt
            //     then the message has been dequeued by another client.

            // Summary:
            //     This value is required to delete the Message. If deletion fails using this popreceipt
            //     then the message has been dequeued by another client.

            // FIXME: TableEntity delegate has PK and RK required for retrieve, QueueMessage delegate hasn't...
            var queueMessage = opt.Value.QueueMessage(message, ctx);

            // FIXME: need message ID and pop receipt - get from receive
            //var deleteResponse = await opt.Value.QueueClient.DeleteMessage(opt.Value.Visibility, cancellationToken);

            //log.LogDebug("Command {Command} completed with status {StatusCode}", this.GetType().Name, deleteResponse.GetRawResponse().Status);
        }
    }
}
