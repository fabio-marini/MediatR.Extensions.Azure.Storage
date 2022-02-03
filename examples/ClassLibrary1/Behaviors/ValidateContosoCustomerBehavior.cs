using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ValidateContosoCustomerBehavior : IPipelineBehavior<ContosoCustomerRequest, Unit>
    {
        private readonly ILogger log;

        public ValidateContosoCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(ContosoCustomerRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            if (string.IsNullOrEmpty(request.MessageId))
            {
                // short-circuit by throwing an exception
                //throw new ArgumentException("MessageId is required! :(");

                log.LogError("MessageId is required! :(");

                // short-circuit by not calling the next behavior
                return Unit.Task;
            }

            log.LogInformation("Behavior {Behavior} completed, invoking next behavior in the chain", this.GetType().Name);

            return next();
        }
    }
}
