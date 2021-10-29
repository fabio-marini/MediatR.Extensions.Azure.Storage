using Azure.Storage.Queues;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class SourceCustomerCommand : IRequest
    {
        public string MessageId { get; set; }
        public SourceCustomer SourceCustomer { get; set; }
        public CanonicalCustomer CanonicalCustomer { get; set; }
    }

    public class SourceCustomerHandler : IRequestHandler<SourceCustomerCommand>
    {
        private readonly QueueClient queueClient;
        private readonly ILogger log;

        public SourceCustomerHandler(QueueClient queueClient, ILogger log = null)
        {
            this.queueClient = queueClient;
            this.log = log ?? NullLogger.Instance;
        }

        public Task<Unit> Handle(SourceCustomerCommand request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request);

            _ = queueClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return Unit.Task;
        }
    }
}
