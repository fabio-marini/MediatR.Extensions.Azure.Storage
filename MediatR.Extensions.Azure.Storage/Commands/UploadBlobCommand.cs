using Azure.Storage.Blobs.Models;
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
        private readonly IOptions<UploadBlobOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public UploadBlobCommand(IOptions<UploadBlobOptions<TMessage>> opt, PipelineContext ctx = null, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public virtual async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // blob name and content are required - defaults will be supplied if not specified
            // a container client is also required, but a default will not be supplied; instead the command will not execute (as if disabled)
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

            var blobClient = opt.Value.BlobClient(message, ctx);

            var blobContent = opt.Value.BlobContent(message, ctx);

            if (blobContent == null)
            {
                // UploadAsync will throw a NullReferenceException if this is null - this message is a bit more helpful hopefully...
                throw new ArgumentNullException($"Command {this.GetType().Name} requires a valid BlobContent value");
            }

            await blobClient.UploadAsync(blobContent, cancellationToken);

            if (opt.Value.BlobHeaders != null)
            {
                var blobHeaders = opt.Value.BlobHeaders(message, ctx);

                await blobClient.SetHttpHeadersAsync(blobHeaders, cancellationToken: cancellationToken);
            }

            if (opt.Value.Metadata != null)
            {
                var blobMetadata = opt.Value.Metadata(message, ctx);

                await blobClient.SetMetadataAsync(blobMetadata, cancellationToken: cancellationToken);
            }
        }
    }
}
