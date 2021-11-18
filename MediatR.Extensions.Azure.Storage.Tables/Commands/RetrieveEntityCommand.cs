using MediatR.Extensions.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class RetrieveEntityCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<TableOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public RetrieveEntityCommand(IOptions<TableOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (opt.Value.IsEnabled == false)
            {
                log.LogDebug("Command {Command} is not enabled, returning", this.GetType().Name);

                return;
            }

            if (opt.Value.CloudTable == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid CloudTable");
            }

            if (opt.Value.TableEntity == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid TableEntity");
            }

            try
            {
                var tableEntity = opt.Value.TableEntity(message, ctx);

                var retrieveOperation = TableOperation.Retrieve(tableEntity.PartitionKey, tableEntity.RowKey);

                var res = await opt.Value.CloudTable.ExecuteAsync(retrieveOperation, cancellationToken);

                if (opt.Value.Retrieved != null)
                {
                    await opt.Value.Retrieved(res, ctx, message);
                }

                log.LogDebug("Command {Command} completed with status {StatusCode}", this.GetType().Name, res.HttpStatusCode);
            }
            catch (Exception ex)
            {
                log.LogDebug(ex, "Command {Command} failed with message: {Message}", this.GetType().Name, ex.Message);

                throw new CommandException($"Command {this.GetType().Name} failed, see inner exception for details", ex);
            }
        }
    }
}
