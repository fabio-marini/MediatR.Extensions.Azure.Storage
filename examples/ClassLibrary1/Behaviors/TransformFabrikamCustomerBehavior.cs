using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class TransformFabrikamCustomerBehavior : IPipelineBehavior<FabrikamCustomerRequest, Unit>
    {
        private readonly ILogger log;

        public TransformFabrikamCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(FabrikamCustomerRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.FabrikamCustomer = new FabrikamCustomer
            {
                FullName = request.CanonicalCustomer.FullName,
                Email = request.CanonicalCustomer.Email
            };

            log.LogInformation("Behavior {Behavior} completed", this.GetType().Name);

            return next();
        }
    }
}
