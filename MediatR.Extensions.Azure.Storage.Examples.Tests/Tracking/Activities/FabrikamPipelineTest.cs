using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.Tracking.Activities
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples.Tests")]
    public class FabrikamPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TableFixture tableFixture;
        private readonly string correlationId;

        public FabrikamPipelineTest(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCoreDependencies(log)
                .AddFabrikamActivityTrackingPipeline()

                .BuildServiceProvider();

            tableFixture = serviceProvider.GetRequiredService<TableFixture>();

            correlationId = "b4702445-613d-4787-b91d-4461c3bd4a4e";
        }

        [Fact(DisplayName = "01. Activities table has entities")]
        public void Step01() => tableFixture.ThenTableHasEntities(2);

        [Fact(DisplayName = "02. Fabrikam pipeline is executed")]
        public async Task Step02()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new FabrikamCustomerRequest
            {
                MessageId = correlationId,
                CanonicalCustomer = new CanonicalCustomer
                {
                    FullName = "Fabio Marini",
                    Email = "fm@example.com"
                }
            };

            var res = await med.Send(req);

            res.MessageId.Should().Be(req.MessageId);
        }

        [Fact(DisplayName = "03. Table has entities")]
        public void Step03() => tableFixture.ThenTableHasEntities(4);

        [Fact(DisplayName = "04. Entities are merged")]
        public void Step04() => tableFixture.ThenEntitiesAreMerged(correlationId);
    }
}
