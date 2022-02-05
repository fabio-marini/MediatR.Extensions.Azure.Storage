using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class FabrikamCustomerRequest : IRequest
    {
        public string MessageId { get; set; }
        public CanonicalCustomer CanonicalCustomer { get; set; }
        public FabrikamCustomer FabrikamCustomer { get; set; }
    }

    public class FabrikamCustomerHandler : IRequestHandler<FabrikamCustomerRequest>
    {
        private readonly DirectoryInfo dir;
        private readonly ILogger log;

        public FabrikamCustomerHandler(DirectoryInfo dir, ILogger log = null)
        {
            this.dir = dir;
            this.log = log ?? NullLogger.Instance;
        }

        public async Task<Unit> Handle(FabrikamCustomerRequest request, CancellationToken cancellationToken)
        {
            var path = Path.Combine(dir.FullName, $"{request.MessageId}.json");

            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(request));

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return Unit.Value;
        }
    }
}
