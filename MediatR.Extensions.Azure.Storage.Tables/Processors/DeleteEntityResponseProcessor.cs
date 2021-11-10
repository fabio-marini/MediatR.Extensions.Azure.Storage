using Microsoft.Extensions.Logging;

namespace MediatR.Extensions.Azure.Storage
{
    public class DeleteEntityResponseProcessor<TRequest, TResponse> : TableResponseProcessorBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public DeleteEntityResponseProcessor(DeleteEntityCommand<TResponse> cmd, PipelineContext ctx = null, ILogger log = null) : base(cmd, ctx, log)
        {
        }
    }
}
