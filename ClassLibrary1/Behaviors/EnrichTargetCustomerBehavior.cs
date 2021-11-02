using MediatR;
using MediatR.Extensions.Azure.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class EnrichTargetCustomerBehavior : IPipelineBehavior<TargetCustomerCommand, Unit>
    {
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public EnrichTargetCustomerBehavior(PipelineContext ctx = null, ILogger log = null)
        {
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(TargetCustomerCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            request.TargetCustomer.DateOfBirth = new DateTime(1970, 10, 26);

            var customerActivity = new CustomerActivityEntity
            {
                PartitionKey = request.MessageId,
                RowKey = Guid.NewGuid().ToString(),
                DateOfBirth = request.TargetCustomer.DateOfBirth,
                CustomerPublishedOn = DateTime.Now
            };

            if (ctx != null)
            {
                ctx.Add("CustomerActivity", customerActivity);
            }

            log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);

            return next();
        }
    }
}
