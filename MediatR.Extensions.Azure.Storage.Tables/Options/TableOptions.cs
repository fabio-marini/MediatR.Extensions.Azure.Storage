using MediatR.Extensions.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace MediatR.Extensions.Azure.Storage
{
    public class TableOptions<TMessage>
    {
        public virtual bool IsEnabled { get; set;  }
        public virtual CloudTable CloudTable { get; set; }
        public virtual Func<TMessage, PipelineContext, ITableEntity> TableEntity { get; set; }

        public virtual Func<TableResult, PipelineContext, TMessage, Task> Retrieved { get; set; }
    }
}
