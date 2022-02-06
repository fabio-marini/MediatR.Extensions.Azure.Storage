using ClassLibrary1;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.ClaimCheck
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples")]
    public class ContosoPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly QueueFixture queueFixture;
        private readonly BlobFixture blobFixture;
        private readonly string correlationId;

        public ContosoPipelineTest(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCoreDependencies(log)
                .AddContosoClaimCheckPipeline()

                .BuildServiceProvider();

            queueFixture = serviceProvider.GetRequiredService<QueueFixture>();

            blobFixture = serviceProvider.GetRequiredService<BlobFixture>();

            correlationId = "5e6d7294-967e-4612-92e0-485aeecdde54";
        }

        [Fact(DisplayName = "01. Queue is empty")]
        public void Step01() => queueFixture.GivenQueueIsEmpty();

        [Fact(DisplayName = "02. Container is empty")]
        public void Step02() => blobFixture.GivenContainerIsEmpty();

        [Fact(DisplayName = "03. Contoso pipeline is executed")]
        public async Task Step03()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new ContosoCustomerRequest
            {
                MessageId = correlationId,
                ContosoCustomer = new ContosoCustomer
                {
                    FirstName = "Fabio",
                    LastName = "Marini",
                    Email = "fm@example.com"
                }
            };

            _ = await med.Send(req);
        }

        [Fact(DisplayName = "04. Container has blobs")]
        public void Step04() => blobFixture.ThenContainerHasBlobs(1);

        [Fact(DisplayName = "05. Queue has messages")]
        public void Step05() => queueFixture.ThenQueueHasMessages(1);
    }
}
