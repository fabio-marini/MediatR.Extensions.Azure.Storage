using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ValidateContosoCustomerBehavior : IPipelineBehavior<ContosoCustomerRequest, ContosoCustomerResponse>
    {
        private readonly ILogger log;

        public ValidateContosoCustomerBehavior(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public Task<ContosoCustomerResponse> Handle(ContosoCustomerRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<ContosoCustomerResponse> next)
        {
            if (string.IsNullOrEmpty(request.MessageId))
            {
                // short-circuit by throwing an exception
                //throw new ArgumentException("MessageId is required! :(");

                log.LogError("MessageId is required! :(");

                // short-circuit by not calling the next behavior
                return default;
            }

            log.LogInformation("Behavior {Behavior} completed", this.GetType().Name);

            return next();
        }
    }
}
