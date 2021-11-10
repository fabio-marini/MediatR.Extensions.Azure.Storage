﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class ReceiveMessageCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<SendMessageOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ReceiveMessageCommand(IOptions<SendMessageOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
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

            var receiveResponse = await opt.Value.QueueClient.ReceiveMessageAsync(opt.Value.Visibility, cancellationToken);

            log.LogDebug("Command {Command} completed with status {StatusCode}", this.GetType().Name, receiveResponse.GetRawResponse().Status);

            if (opt.Value.Received != null)
            {
                await opt.Value.Received(receiveResponse.Value, ctx, message);
            }
        }
    }
}
