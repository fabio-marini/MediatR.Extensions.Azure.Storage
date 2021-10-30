using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobOptions<TRequest> : UploadBlobOptions<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    public class UploadBlobOptions<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual Func<TRequest, PipelineContext, BlobClient> BlobClient { get; set; }
        public virtual Func<TRequest, PipelineContext, BinaryData> BlobContent { get; set; }
        public virtual Func<TRequest, PipelineContext, BlobHttpHeaders> BlobHeaders { get; set; }
        public virtual Func<TRequest, PipelineContext, Dictionary<string, string>> Metadata { get; set; }
    }
}
