using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class RetrieveEntityResponseProcessor<TRequest, TResponse> : TableResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public RetrieveEntityResponseProcessor(RetrieveEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
