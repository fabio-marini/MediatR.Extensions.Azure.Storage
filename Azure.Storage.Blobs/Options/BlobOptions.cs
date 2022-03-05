using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR.Extensions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class BlobOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set; }

        public virtual Func<TMessage, PipelineContext, BlobClient> BlobClient { get; set; }
        public virtual Func<TMessage, PipelineContext, BinaryData> BlobContent { get; set; }
        public virtual Func<TMessage, PipelineContext, BlobHttpHeaders> BlobHeaders { get; set; }
        public virtual Func<TMessage, PipelineContext, Dictionary<string, string>> Metadata { get; set; }

        // the event that is raised after the blob is downloaded (allows using the blob to modify TMessage)
        public virtual Func<BlobDownloadResult, PipelineContext, TMessage, Task> Downloaded { get; set; }
    }
}
