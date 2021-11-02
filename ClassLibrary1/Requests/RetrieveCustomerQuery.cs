using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class RetrieveCustomerQuery : IRequest<RetrieveCustomerResult>
    {
        public string MessageId { get; set; }
    }

    public class RetrieveCustomerResult
    {
        public string MessageId { get; set; }

        public TargetCustomer Customer { get; set; }
    }

    public class RetrieveCustomerHandler : IRequestHandler<RetrieveCustomerQuery, RetrieveCustomerResult>
    {
        private readonly ILogger log;

        public RetrieveCustomerHandler(ILogger log = null)
        {
            this.log = log;
        }

        public async Task<RetrieveCustomerResult> Handle(RetrieveCustomerQuery request, CancellationToken cancellationToken)
        {
            var json = await File.ReadAllTextAsync($"C:\\Repos\\Customers\\{request.MessageId}.json");

            var result = new RetrieveCustomerResult
            {
                MessageId = request.MessageId,
                Customer = JsonConvert.DeserializeObject<TargetCustomerCommand>(json).TargetCustomer
            };

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return result;
        }
    }
}
