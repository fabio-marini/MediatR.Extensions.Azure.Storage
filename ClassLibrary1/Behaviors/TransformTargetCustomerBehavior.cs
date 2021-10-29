using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class TransformTargetCustomerBehavior : IPipelineBehavior<TargetCustomerCommand, Unit>
    {
        private readonly ILogger log;

        public TransformTargetCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(TargetCustomerCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.TargetCustomer = new TargetCustomer
            {
                FullName = request.CanonicalCustomer.FullName,
                Email = request.CanonicalCustomer.Email
            };

            log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);

            return next();
        }
    }
}
