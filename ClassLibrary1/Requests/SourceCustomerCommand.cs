﻿using Azure.Storage.Queues;
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

        public async Task<Unit> Handle(SourceCustomerCommand request, CancellationToken cancellationToken)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(request, settings);

            _ = await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));

            log.LogInformation("Handler {Handler} completed, returning", this.GetType().Name);

            return Unit.Value;
        }
    }
}
