using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class TransformContosoCustomerBehavior : IPipelineBehavior<ContosoCustomerRequest, Unit>
    {
        private readonly ILogger log;

        public TransformContosoCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(ContosoCustomerRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.CanonicalCustomer = new CanonicalCustomer
            {
                FullName = $"{request.ContosoCustomer.FirstName} {request.ContosoCustomer.LastName}",
                Email = request.ContosoCustomer.Email
            };

            log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);

            return next();
        }
    }
}
