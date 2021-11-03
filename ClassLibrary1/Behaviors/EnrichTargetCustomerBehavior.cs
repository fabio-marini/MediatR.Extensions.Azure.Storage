using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class EnrichTargetCustomerBehavior : IPipelineBehavior<TargetCustomerCommand, Unit>
    {
        private readonly ILogger log;

        public EnrichTargetCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(TargetCustomerCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.TargetCustomer.DateOfBirth = new DateTime(1970, 10, 26);

            log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);

            return next();
        }
    }
}
