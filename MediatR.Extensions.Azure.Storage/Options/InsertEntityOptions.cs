using Microsoft.Azure.Cosmos.Table;
using System;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityOptions<TRequest> where TRequest : IRequest
    {
        public virtual bool IsEnabled { get; set; }
        public virtual CloudTable CloudTable { get; set; }
        public virtual Func<TRequest, PipelineContext, ITableEntity> TableEntity { get; set; }
    }
}
