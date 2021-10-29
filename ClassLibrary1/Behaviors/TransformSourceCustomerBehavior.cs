using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class TransformSourceCustomerBehavior : IPipelineBehavior<SourceCustomerCommand, Unit>
    {
        private readonly ILogger log;

        public TransformSourceCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(SourceCustomerCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.CanonicalCustomer = new CanonicalCustomer
            {
                FullName = $"{request.SourceCustomer.FirstName} {request.SourceCustomer.LastName}",
                Email = request.SourceCustomer.Email
            };

            log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);

            return next();
        }
    }
}
