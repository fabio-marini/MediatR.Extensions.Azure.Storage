using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace MediatR.Extensions.Azure.Storage
{
    public class PipelineContext : Dictionary<string, object>
    {
        // https://jimmybogard.com/sharing-context-in-mediatr-pipelines/
        public PipelineContext()
        {
            PipelineId = Guid.NewGuid().ToString();
            Exceptions = new List<Exception>();

            Entities = new List<DynamicTableEntity>();
            Blobs = new List<BlobDownloadResult>();
            Messages = new List<QueueMessage>();
        }

        public virtual string PipelineId { get; }
        public virtual List<Exception> Exceptions { get; }

        // these are used by the retrieve extensions to store the retrieved values
        public virtual List<DynamicTableEntity> Entities { get; }
        public virtual List<BlobDownloadResult> Blobs { get; }
        public virtual List<QueueMessage> Messages { get; }
    }
}
