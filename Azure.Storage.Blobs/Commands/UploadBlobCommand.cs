using Azure.Storage.Blobs.Models;
using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<BlobOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public UploadBlobCommand(IOptions<BlobOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
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

            if (opt.Value.BlobClient == null)
            {
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid BlobClient");
            }

            if (opt.Value.BlobContent == null)
            {
                log.LogDebug("Command {Command} is using the default BlobContent delegate", this.GetType().Name);

                opt.Value.BlobContent = (req, ctx) =>
                {
                    var json = JsonConvert.SerializeObject(req);

                    return BinaryData.FromString(json);
                };

                if (opt.Value.BlobHeaders == null)
                {
                    log.LogDebug("Command {Command} is using the default BlobHeaders delegate", this.GetType().Name);

                    opt.Value.BlobHeaders = (req, ctx) => new BlobHttpHeaders { ContentType = "application/json" };
                }
            }

            try
            {
                var blobClient = opt.Value.BlobClient(message, ctx);

                var blobContent = opt.Value.BlobContent(message, ctx);

                var res = await blobClient.UploadAsync(blobContent, cancellationToken);

                if (opt.Value.BlobHeaders != null)
                {
                    var blobHeaders = opt.Value.BlobHeaders(message, ctx);

                    await blobClient.SetHttpHeadersAsync(blobHeaders, cancellationToken: cancellationToken);

                    log.LogDebug("Command {Command} set the specified blob HTTP headers", this.GetType().Name);
                }

                if (opt.Value.Metadata != null)
                {
                    var blobMetadata = opt.Value.Metadata(message, ctx);

                    await blobClient.SetMetadataAsync(blobMetadata, cancellationToken: cancellationToken);

                    log.LogDebug("Command {Command} set the specified blob metadata", this.GetType().Name);
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
