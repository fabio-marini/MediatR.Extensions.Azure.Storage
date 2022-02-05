using Azure.Storage.Blobs;
using ClassLibrary1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.Tracking.Messages
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples")]
    public class ContosoPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly QueueFixture queueFixture;
        private readonly BlobFixture blobFixture;

        public ContosoPipelineTest(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCore()
                .AddTransient<BlobContainerClient>(sp =>
                {
                    var blobclient = new BlobContainerClient("UseDevelopmentStorage=true", "integration-tests");
                    blobclient.CreateIfNotExists();

                    return blobclient;
                })

                .AddTransient<ITestOutputHelper>(sp => log)
                .AddTransient<ILogger, TestOutputLogger>()
                .AddTransient<BlobFixture>()
                .AddTransient<QueueFixture>()

                .AddSingleton<IConfiguration>(sp =>
                {
                    var appSettings = new Dictionary<string, string>
                    {
                        { "TrackingEnabled", "true" }
                    };

                    return new ConfigurationBuilder()

                        .AddInMemoryCollection(appSettings)
                        .Build();
                })
                .AddContosoMessageTrackingPipeline()

                .BuildServiceProvider();

            queueFixture = serviceProvider.GetRequiredService<QueueFixture>();

            blobFixture = serviceProvider.GetRequiredService<BlobFixture>();
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
                MessageId = Guid.NewGuid().ToString(),
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
        public void Step04() => blobFixture.ThenContainerHasBlobs(2);

        [Fact(DisplayName = "05. Queue has messages")]
        public void Step05() => queueFixture.ThenQueueHasMessages(1);
    }
}
