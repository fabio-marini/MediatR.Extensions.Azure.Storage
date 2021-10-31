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
    public class UploadMessageCommand<TMessage> : ICommand<TMessage>
    {
        private readonly IOptions<UploadBlobOptions<TMessage>> opt;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public UploadMessageCommand(IOptions<UploadBlobOptions<TMessage>> opt, PipelineContext ctx, ILogger log = null)
        {
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken)
        {
            // blob name and content are required - defaults will be supplied if not specified
            // a container client is also required, but a default will not be supplied; instead the command will not execute (as if disabled)
            if (opt.Value.IsEnabled == false)
            {
                // command is disabled - skip
                log.LogDebug("Command {Command} is not enabled, returning", this.GetType().Name);

                return;
            }

            if (opt.Value.BlobClient == null)
            {
                // no BlobClient configured - skip
                log.LogError("Command {Command} requires a valid BlobClient", this.GetType().Name);

                return;
            }

            if (opt.Value.BlobContent == null)
            {
                // command is enabled, but no BlobContent func specified - use default
                log.LogDebug("Command {Command} is using the default BlobContent delegate", this.GetType().Name);

                opt.Value.BlobContent = (req, ctx) =>
                {
                    var json = JsonConvert.SerializeObject(req);

                    return BinaryData.FromString(json);
                };

                if (opt.Value.BlobHeaders == null)
                {
                    // command is enabled, but no BlobHeaders func specified - use default
                    log.LogDebug("Command {Command} is using the default BlobHeaders delegate", this.GetType().Name);

                    opt.Value.BlobHeaders = (req, ctx) => new BlobHttpHeaders { ContentType = "application/json" };
                }
            }

            try
            {
                var blobClient = opt.Value.BlobClient(message, ctx);

                var blobContent = opt.Value.BlobContent(message, ctx);

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

                log.LogInformation("Command {Command} completed, returning", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Command {Command} failed, returning", this.GetType().Name);
            }
        }
    }
}
