using MediatR;
using MediatR.Extensions.Azure.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ValidateSourceCustomerBehavior : IPipelineBehavior<SourceCustomerCommand, Unit>
    {
        private readonly PipelineContext ctx;
        private readonly ILogger log;

        public ValidateSourceCustomerBehavior(PipelineContext ctx = null, ILogger log = null)
        {
            this.ctx = ctx;
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(SourceCustomerCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            if (string.IsNullOrEmpty(request.MessageId))
            {
                // short-circuit by throwing an exception
                //throw new ArgumentException("MessageId is required! :(");

                log.LogError("MessageId is required! :(");

                // short-circuit by not calling the next behavior
                return Unit.Task;
            }

            var customerActivity = new CustomerActivityEntity
            {
                PartitionKey = request.MessageId,
                RowKey = Guid.NewGuid().ToString(),
                IsValid = true,
                CustomerReceivedOn = DateTime.Now,
                Email = request.SourceCustomer.Email
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
