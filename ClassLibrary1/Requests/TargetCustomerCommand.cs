using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class TargetCustomerCommand : IRequest
    {
        public string MessageId { get; set; }
        public CanonicalCustomer CanonicalCustomer { get; set; }
        public TargetCustomer TargetCustomer { get; set; }
    }

    public class TargetCustomerHandler : IRequestHandler<TargetCustomerCommand>
    {
        private readonly ILogger log;

        public TargetCustomerHandler(ILogger log = null)
        {
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<Unit> Handle(TargetCustomerCommand request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request);

            await File.WriteAllTextAsync($"C:\\Repos\\Customers\\{request.MessageId}.json", json);

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return Unit.Value;
        }
    }
}
