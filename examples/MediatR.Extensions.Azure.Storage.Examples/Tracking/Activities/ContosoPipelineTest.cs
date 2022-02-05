using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.Tracking.Activities
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples")]
    public class ContosoPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TableFixture tableFixture;
        private readonly QueueFixture queueFixture;

        public ContosoPipelineTest(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCore()
                .AddTransient<CloudTable>(sp =>
                {
                    var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

                    var cloudTable = storageAccount.CreateCloudTableClient().GetTableReference("IntegrationTests");
                    cloudTable.CreateIfNotExists();

                    return cloudTable;
                })

                .AddTransient<ITestOutputHelper>(sp => log)
                .AddTransient<ILogger, TestOutputLogger>()
                .AddTransient<QueueFixture>()
                .AddTransient<TableFixture>()

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
                .AddContosoActivityTrackingPipeline()

                .BuildServiceProvider();

            tableFixture = serviceProvider.GetRequiredService<TableFixture>();

            queueFixture = serviceProvider.GetRequiredService<QueueFixture>();
        }

        [Fact(DisplayName = "01. Queue is empty")]
        public void Step01() => queueFixture.GivenQueueIsEmpty();

        [Fact(DisplayName = "02. Table is empty")]
        public void Step02() => tableFixture.GivenTableIsEmpty();

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

        [Fact(DisplayName = "04. Table has entities")]
        public void Step04() => tableFixture.ThenTableHasEntities(1);

        [Fact(DisplayName = "05. Queue has messages")]
        public void Step05() => queueFixture.ThenQueueHasMessages(1);
    }
}
