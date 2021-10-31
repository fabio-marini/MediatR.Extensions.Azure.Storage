using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual Func<TMessage, PipelineContext, BlobClient> BlobClient { get; set; }
        public virtual Func<TMessage, PipelineContext, BinaryData> BlobContent { get; set; }
        public virtual Func<TMessage, PipelineContext, BlobHttpHeaders> BlobHeaders { get; set; }
        public virtual Func<TMessage, PipelineContext, Dictionary<string, string>> Metadata { get; set; }
    }
}
