using Azure.Storage.Blobs;
using MediatR;
using MediatR.Extensions.Azure.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ClaimCheckTargetCustomerBehavior : IPipelineBehavior<TargetCustomerCommand, Unit>
    {
        private readonly Func<TargetCustomerCommand, PipelineContext, BlobClient> blb;
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ClaimCheckTargetCustomerBehavior(Func<TargetCustomerCommand, PipelineContext, BlobClient> blb, 
            PipelineContext ctx = null, ILogger log = null)
        {
            this.blb = blb;
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<Unit> Handle(TargetCustomerCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            var blobClient = blb(request, ctx);

            using var ms = new MemoryStream();

            await blobClient.DownloadToAsync(ms, cancellationToken);

            request.CanonicalCustomer = JsonConvert.DeserializeObject<CanonicalCustomer>(Encoding.UTF8.GetString(ms.ToArray()));

            return await next();
        }
    }
}
