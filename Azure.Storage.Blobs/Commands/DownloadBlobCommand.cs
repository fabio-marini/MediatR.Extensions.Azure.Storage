using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class DownloadBlobCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<BlobOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public DownloadBlobCommand(IOptions<BlobOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
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

            if (opt.Value.BlobClient == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid BlobClient");
            }

            try
            {
                var blobClient = opt.Value.BlobClient(message, ctx);

                var res = await blobClient.DownloadContentAsync(cancellationToken);

                if (opt.Value.Downloaded != null)
                {
                    await opt.Value.Downloaded(res, ctx, message);
                }

                log.LogDebug("Command {Command} completed with status {Status}", this.GetType().Name, res.GetRawResponse().Status);
            }
            catch (Exception ex)
            {
                log.LogDebug(ex, "Command {Command} failed with message: {Message}", this.GetType().Name, ex.Message);

                throw new CommandException($"Command {this.GetType().Name} failed, see inner exception for details", ex);
            }
        }
    }
}
