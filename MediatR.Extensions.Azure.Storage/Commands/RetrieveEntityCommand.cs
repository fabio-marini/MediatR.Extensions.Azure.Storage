﻿using Microsoft.Azure.Cosmos.Table;
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
        private readonly IOptions<InsertEntityOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public RetrieveEntityCommand(IOptions<InsertEntityOptions<TMessage>> opt, PipelineContext ctx, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx ?? throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid PipelineContext");
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

            var tableEntity = opt.Value.TableEntity(message, ctx);

            if (tableEntity == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid TableEntity value");
            }

            var retrieveOperation = TableOperation.Retrieve(tableEntity.PartitionKey, tableEntity.RowKey);

            var tableResult = await opt.Value.CloudTable.ExecuteAsync(retrieveOperation, cancellationToken);

            log.LogDebug("Command {Command} completed with status {StatusCode}", this.GetType().Name, tableResult.HttpStatusCode);

            var dte = tableResult.Result as DynamicTableEntity;

            if (dte != null)
            {
                ctx.Entities.Add(dte);

                if (opt.Value.Select != null)
                {
                    await opt.Value.Select(dte, ctx, message);
                }
            }
        }
    }
}
