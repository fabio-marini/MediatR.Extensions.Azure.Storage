using Microsoft.Azure.Cosmos.Table;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public class TableFixture
    {
        public TableFixture()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            CloudTable = storageAccount.CreateCloudTableClient().GetTableReference("IntegrationTests");
            CloudTable.CreateIfNotExists();
        }

        public CloudTable CloudTable { get; }
    }
}
