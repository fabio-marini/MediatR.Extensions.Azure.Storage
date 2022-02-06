using ClassLibrary1;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.Tracking.Messages
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples")]
    public class FabrikamPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly FolderFixture folderFixture;
        private readonly BlobFixture blobFixture;

        public FabrikamPipelineTest(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCoreDependencies(log)
                .AddFabrikamMessageTrackingPipeline()

                .BuildServiceProvider();

            folderFixture = serviceProvider.GetRequiredService<FolderFixture>();

            blobFixture = serviceProvider.GetRequiredService<BlobFixture>();
        }

        [Fact(DisplayName = "01. Folder is empty")]
        public void Step01() => folderFixture.GivenFolderIsEmpty();

        [Fact(DisplayName = "02. Container is empty")]
        public void Step02() => blobFixture.GivenContainerIsEmpty();

        [Fact(DisplayName = "03. Fabrikam pipeline is executed")]
        public async Task Step03()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new FabrikamCustomerRequest
            {
                MessageId = Guid.NewGuid().ToString(),
                CanonicalCustomer = new CanonicalCustomer
                {
                    FullName = "Fabio Marini",
                    Email = "fm@example.com"
                }
            };

            _ = await med.Send(req);
        }

        [Fact(DisplayName = "04. Container has blobs")]
        public void Step04() => blobFixture.ThenContainerHasBlobs(2);

        [Fact(DisplayName = "05. Folder has files")]
        public void Step05() => folderFixture.ThenFolderHasFiles(1);
    }
}
