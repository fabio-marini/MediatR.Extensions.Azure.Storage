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
    public class ProcessCustomer
    {
        private readonly IMediator mediator;

        public ProcessCustomer(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [FunctionName(nameof(HttpSourceCustomer))]
        public async Task<IActionResult> HttpSourceCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "customers")] ContosoCustomer sourceCustomer)
        {
            var cmd = new ContosoCustomerRequest
            {
                MessageId = Guid.NewGuid().ToString(),
                ContosoCustomer = sourceCustomer
            };

            _ = await mediator.Send(cmd);

            return new AcceptedResult(cmd.MessageId, null);
        }

        [FunctionName(nameof(QueueSourceCustomer))]
        public async Task QueueSourceCustomer([QueueTrigger("customers")] string queueMessage)
        {
            var msg = JsonConvert.DeserializeObject<ContosoCustomerRequest>(queueMessage);

            var cmd = new FabrikamCustomerRequest
            {
                // this is used to correlate source and target commands
                MessageId = msg.MessageId,
                //CanonicalCustomer = msg.CanonicalCustomer
            };

            _ = await mediator.Send(cmd);
        }

        [FunctionName(nameof(HttpTargetCustomer))]
        public async Task<IActionResult> HttpTargetCustomer(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "customers/{messageId}")]HttpRequest req, string messageId)
        {
            throw new Exception("Removed RetrieveCustomerRequest! :)");
            //var qry = new RetrieveCustomerRequest
            //{
            //    MessageId = messageId,
            //};

            //try
            //{
            //    //var src = new CancellationTokenSource(500);

            //    var res = await mediator.Send(qry);

            //    return new OkObjectResult(res);
            //}
            //catch (OperationCanceledException)
            //{
            //    return new StatusCodeResult(408);
            //}
        }
    }
}
