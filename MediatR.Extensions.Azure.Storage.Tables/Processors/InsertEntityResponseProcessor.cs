using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertEntityResponseProcessor<TRequest, TResponse> : TableResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public InsertEntityResponseProcessor(InsertEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
