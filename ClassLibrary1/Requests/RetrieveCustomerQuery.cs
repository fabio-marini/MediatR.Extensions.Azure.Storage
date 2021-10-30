using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class RetrieveCustomerQuery : IRequest<TargetCustomer>
    {
        public string MessageId { get; set; }
    }

    public class RetrieveCustomerHandler : IRequestHandler<RetrieveCustomerQuery, TargetCustomer>
    {
        private readonly ILogger log;

        public RetrieveCustomerHandler(ILogger log = null)
        {
            this.log = log;
        }

        public Task<TargetCustomer> Handle(RetrieveCustomerQuery request, CancellationToken cancellationToken)
        {
            var json = File.ReadAllText($"C:\\Repos\\Customers\\{request.MessageId}.json");

            var targetCustomer = JsonConvert.DeserializeObject<TargetCustomerCommand>(json);

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return Task.FromResult(targetCustomer.TargetCustomer);
        }
    }
}
