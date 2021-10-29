﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;

namespace MediatR.Extensions.Azure.Storage
{
    public class UploadBlobOptions<TRequest> where TRequest : IRequest
    {
        public virtual bool IsEnabled { get; set; }
        public virtual BlobContainerClient Container { get; set; }
        public virtual Func<TRequest, PipelineContext, string> BlobName { get; set; }
        public virtual Func<TRequest, PipelineContext, BinaryData> BlobContent { get; set; }
        public virtual Func<TRequest, PipelineContext, BlobHttpHeaders> BlobHeaders{ get; set; }
        public virtual Func<TRequest, PipelineContext, Dictionary<string, string>> Metadata { get; set; }
    }
}
