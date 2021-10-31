using Microsoft.Azure.Cosmos.Table;
using System;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set;  }
        public virtual CloudTable CloudTable { get; set; }
        public virtual Func<TMessage, PipelineContext, ITableEntity> TableEntity { get; set; }
    }
}
