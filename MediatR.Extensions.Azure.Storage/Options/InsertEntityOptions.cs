using Microsoft.Azure.Cosmos.Table;
using System;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityOptions<TRequest> : InsertEntityOptions<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    public class InsertEntityOptions<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public virtual bool IsEnabled { get; set; }
        public virtual CloudTable CloudTable { get; set; }
        public virtual Func<TRequest, PipelineContext, ITableEntity> TableEntity { get; set; }
    }
}
