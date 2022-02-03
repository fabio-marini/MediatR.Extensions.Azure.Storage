using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class RetrieveCustomerRequest : IRequest<RetrieveCustomerResponse>
    {
        public string MessageId { get; set; }
    }

    public class RetrieveCustomerResponse
    {
        public string MessageId { get; set; }

        public FabrikamCustomer Customer { get; set; }
    }

    public class RetrieveCustomerHandler : IRequestHandler<RetrieveCustomerRequest, RetrieveCustomerResponse>
    {
        private readonly ILogger log;

        public RetrieveCustomerHandler(ILogger log = null)
        {
            this.log = log;
        }

        public async Task<RetrieveCustomerResponse> Handle(RetrieveCustomerRequest request, CancellationToken cancellationToken)
        {
            var json = await File.ReadAllTextAsync($"C:\\Repos\\Customers\\{request.MessageId}.json");

            var result = new RetrieveCustomerResponse
            {
                MessageId = request.MessageId,
                Customer = JsonConvert.DeserializeObject<FabrikamCustomerRequest>(json).FabrikamCustomer
            };

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return result;
        }
    }
}
