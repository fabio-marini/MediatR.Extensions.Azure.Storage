using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual Func<TMessage, PipelineContext, BlobClient> BlobClient { get; set; }
        public virtual Func<TMessage, PipelineContext, BinaryData> BlobContent { get; set; }
        public virtual Func<TMessage, PipelineContext, BlobHttpHeaders> BlobHeaders { get; set; }
        public virtual Func<TMessage, PipelineContext, Dictionary<string, string>> Metadata { get; set; }

        // (optional) use the downloaded blob to update the message
        public virtual Func<BlobDownloadResult, PipelineContext, TMessage, Task> Select { get; set; }
    }
}
