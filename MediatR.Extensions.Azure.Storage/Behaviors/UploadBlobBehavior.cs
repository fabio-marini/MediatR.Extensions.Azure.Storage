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
    public class UploadBlobBehavior<TRequest> : UploadBlobBehavior<TRequest, Unit> where TRequest : IRequest<Unit>
    {
        public UploadBlobBehavior(IOptions<UploadBlobOptions<TRequest>> opt, PipelineContext ctx, ILogger log = null)
            : base(opt, ctx, log)
        {
        }
    }

    public class UploadBlobBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger log;
        private readonly IOptions<UploadBlobOptions<TRequest, TResponse>> opt;
        private readonly PipelineContext ctx;

        public UploadBlobBehavior(IOptions<UploadBlobOptions<TRequest, TResponse>> opt, PipelineContext ctx, ILogger log = null)
        {
            // this parameter is required: if an instance is not supplied, it will be created using the default ctor
            // (which will set IsEnabled = false) - no additional validation is required...
            this.opt = opt;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // blob name and content are required - defaults will be supplied if not specified
            // a container client is also required, but a default will not be supplied; instead the behavior will not execute (as if disabled)
            if (opt.Value.IsEnabled == false)
            {
                // behavior is disabled - skip
                log.LogDebug("Behavior {Behavior} is not enabled, invoking next behavior in the chain", this.GetType().Name);

                return await next();
            }

            if (opt.Value.BlobClient == null)
            {
                // no BlobClient configured - skip
                log.LogError("Behavior {Behavior} requires a valid BlobClient", this.GetType().Name);

                return await next();
            }

            if (opt.Value.BlobContent == null)
            {
                // behavior is enabled, but no BlobContent func specified - use default
                log.LogDebug("Behavior {Behavior} is using the default BlobContent delegate", this.GetType().Name);

                opt.Value.BlobContent = (req, ctx) =>
                {
                    var json = JsonConvert.SerializeObject(req);

                    return BinaryData.FromString(json);
                };

                if (opt.Value.BlobHeaders == null)
                {
                    // behavior is enabled, but no BlobHeaders func specified - use default
                    log.LogDebug("Behavior {Behavior} is using the default BlobHeaders delegate", this.GetType().Name);

                    opt.Value.BlobHeaders = (req, ctx) => new BlobHttpHeaders { ContentType = "application/json" };
                }
            }

            try
            {
                var blobClient = opt.Value.BlobClient(request, ctx);

                var blobContent = opt.Value.BlobContent(request, ctx);

                await blobClient.UploadAsync(blobContent, cancellationToken);

                if (opt.Value.BlobHeaders != null)
                {
                    var blobHeaders = opt.Value.BlobHeaders(request, ctx);

                    await blobClient.SetHttpHeadersAsync(blobHeaders, cancellationToken: cancellationToken);
                }

                if (opt.Value.Metadata != null)
                {
                    var blobMetadata = opt.Value.Metadata(request, ctx);

                    await blobClient.SetMetadataAsync(blobMetadata, cancellationToken: cancellationToken);
                }

                log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);
            }
            catch (Exception ex)
            {
                // failure should not stop execution - log exception, but don't rethrow
                log.LogError(ex, "Behavior {Behavior} failed, invoking next behavior in the chain", this.GetType().Name);
            }

            return await next();
        }
    }
}
