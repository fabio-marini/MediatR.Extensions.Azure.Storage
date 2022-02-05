using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class EnrichFabrikamCustomerBehavior : IPipelineBehavior<FabrikamCustomerRequest, Unit>
    {
        private readonly ILogger log;

        public EnrichFabrikamCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(FabrikamCustomerRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.FabrikamCustomer.DateOfBirth = new DateTime(1970, 10, 26);

            log.LogInformation("Behavior {Behavior} completed", this.GetType().Name);

            return next();
        }
    }
}
