using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<InsertEntityOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public InsertEntityCommand(IOptions<InsertEntityOptions<TMessage>> opt, PipelineContext ctx, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            if (opt.Value.IsEnabled == false)
            {
                // command is disabled - skip
                log.LogDebug("Command {Command} is not enabled, returning", this.GetType().Name);

                return;
            }

            if (opt.Value.CloudTable == null)
            {
                // no table configured - skip
                log.LogError("Command {Command} requires a valid CloudTable", this.GetType().Name);

                return;
            }

            if (opt.Value.TableEntity == null)
            {
                // command is enabled, but no TableEntity func specified - use default
                log.LogDebug("Command {Command} is using the default TableEntity delegate", this.GetType().Name);

                opt.Value.TableEntity = (msg, ctx) =>
                {
                    var pk = Guid.NewGuid().ToString();
                    var rk = Guid.NewGuid().ToString();

                    var tableEntity = new DynamicTableEntity(pk, rk);

                    tableEntity.Properties.Add("Message", EntityProperty.GeneratePropertyForString(msg.GetType().FullName));
                    tableEntity.Properties.Add("Content", EntityProperty.GeneratePropertyForString(JsonConvert.SerializeObject(msg)));

                    return tableEntity;
                };
            }

            try
            {
                var tableEntity = opt.Value.TableEntity(message, ctx);

                var insertOperation = TableOperation.Insert(tableEntity);

                await opt.Value.CloudTable.ExecuteAsync(insertOperation, cancellationToken);

                log.LogInformation("Command {Command} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Command {Command} failed, returning", this.GetType().Name);
            }

            return;
        }
    }
}
