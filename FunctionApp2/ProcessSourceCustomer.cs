using ClassLibrary1;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace FunctionApp2
{
    public class ProcessSourceCustomer
    {
        private readonly IMediator mediator;

        public ProcessSourceCustomer(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [FunctionName(nameof(HttpSourceCustomer))]
        public async Task<IActionResult> HttpSourceCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "customers")] SourceCustomer sourceCustomer)
        {
            var cmd = new SourceCustomerCommand
            {
                MessageId = Guid.NewGuid().ToString(),
                SourceCustomer = sourceCustomer
            };

            _ = await mediator.Send(cmd);

            return new OkResult();
        }

        [FunctionName(nameof(QueueSourceCustomer))]
        public async Task QueueSourceCustomer([QueueTrigger("customers")] string queueMessage)
        {
            var msg = JsonConvert.DeserializeObject<SourceCustomerCommand>(queueMessage);

            var cmd = new TargetCustomerCommand
            {
                // this is used to correlate source and target commands
                MessageId = msg.MessageId,
                CanonicalCustomer = msg.CanonicalCustomer
            };

            _ = await mediator.Send(cmd);
        }

        [FunctionName(nameof(HttpTargetCustomer))]
        public async Task<IActionResult> HttpTargetCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "customers/{messageId}")]HttpRequest req, string messageId)
        {
            var qry = new RetrieveCustomerQuery
            {
                MessageId = messageId,
            };

            var res = await mediator.Send(qry);

            return new OkObjectResult(res);
        }
    }
}
