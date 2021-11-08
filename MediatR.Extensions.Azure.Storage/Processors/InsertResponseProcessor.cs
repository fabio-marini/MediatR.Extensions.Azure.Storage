using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class InsertResponseProcessor<TRequest, TResponse> : ResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public InsertResponseProcessor(InsertEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
