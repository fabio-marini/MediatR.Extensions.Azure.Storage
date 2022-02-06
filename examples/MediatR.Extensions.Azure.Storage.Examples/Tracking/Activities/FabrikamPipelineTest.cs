using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples.Tracking.Activities
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Examples")]
    public class FabrikamPipelineTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly FolderFixture folderFixture;
        private readonly TableFixture tableFixture;
        private readonly string correlationId;

        public FabrikamPipelineTest(ITestOutputHelper log)
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
                .AddTransient<TableFixture>()
                .AddTransient<FolderFixture>()

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
                .AddFabrikamActivityTrackingPipeline()

                .BuildServiceProvider();

            folderFixture = serviceProvider.GetRequiredService<FolderFixture>();

            tableFixture = serviceProvider.GetRequiredService<TableFixture>();

            correlationId = "b4702445-613d-4787-b91d-4461c3bd4a4e";
        }

        [Fact(DisplayName = "01. Folder is empty")]
        public void Step01() => folderFixture.GivenFolderIsEmpty();

        [Fact(DisplayName = "02. Table is empty", Skip = "Leave entities for merge")]
        public void Step02() => tableFixture.GivenTableIsEmpty();

        [Fact(DisplayName = "03. Fabrikam pipeline is executed")]
        public async Task Step03()
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

            _ = await med.Send(req);
        }

        [Fact(DisplayName = "04. Table has entities")]
        public void Step04() => tableFixture.ThenTableHasEntities(4);

        [Fact(DisplayName = "05. Folder has files")]
        public void Step05() => folderFixture.ThenFolderHasFiles(1);

        [Fact(DisplayName = "06. Entities are merged")]
        public void Step06() => tableFixture.ThenEntitiesAreMerged(correlationId);
    }
}
