using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public abstract class TableResponseBehaviorBase<TRequest, TResponse> : ResponseBehaviorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TableResponseBehaviorBase(InsertEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public TableResponseBehaviorBase(RetrieveEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }

        public TableResponseBehaviorBase(DeleteEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
