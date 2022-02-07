using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.ClaimCheck
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples.Tests")]
    public class ContosoPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly BlobFixture blobFixture;
        private readonly string correlationId;

        public ContosoPipelineTest(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCoreDependencies(log)
                .AddContosoClaimCheckPipeline()

                .BuildServiceProvider();

            blobFixture = serviceProvider.GetRequiredService<BlobFixture>();

            correlationId = "5e6d7294-967e-4612-92e0-485aeecdde54";
        }

        [Fact(DisplayName = "01. Claim checks container is empty")]
        public void Step01() => blobFixture.GivenContainerIsEmpty();

        [Fact(DisplayName = "02. Contoso pipeline is executed")]
        public async Task Step02()
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

            var res = await med.Send(req);

            res.MessageId.Should().Be(req.MessageId);
        }

        [Fact(DisplayName = "03. Claim checks container has blobs")]
        public void Step03() => blobFixture.ThenContainerHasBlobs(1);
    }
}
