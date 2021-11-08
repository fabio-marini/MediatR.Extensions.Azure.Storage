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

            // use PartitionKey and RowKey to identify the entity
            Entities = new List<DynamicTableEntity>();

            // use the blob name as the key to identify the blob
            Blobs = new Dictionary<string, BlobDownloadResult>();

            // just use Enqueue() and Dequeue()...
            Messages = new Queue<QueueMessage>();
        }

        public virtual string PipelineId { get; }
        public virtual List<Exception> Exceptions { get; }

        // these are used by the retrieve extensions to store the retrieved values
        public virtual List<DynamicTableEntity> Entities { get; }
        public virtual Dictionary<string, BlobDownloadResult> Blobs { get; }
        public virtual Queue<QueueMessage> Messages { get; }
    }
}
