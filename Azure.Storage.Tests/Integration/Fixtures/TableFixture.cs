using FluentAssertions;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;

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

        public void GivenTableIsEmpty()
        {
            var allEntities = CloudTable.ExecuteQuery(new TableQuery());

            if (allEntities.Any() == false)
            {
                return;
            }

            foreach (var e in allEntities)
            {
                var op = TableOperation.Delete(e);

                var res = CloudTable.Execute(op);

                res.HttpStatusCode.Should().Be(204);
            }
        }

        public void ThenTableIsEmpty()
        {
            var allEntities = CloudTable.ExecuteQuery(new TableQuery());

            allEntities.Any().Should().BeFalse();
        }

        public void ThenTableHasEntities(int entityCount)
        {
            var allEntities = CloudTable.ExecuteQuery(new TableQuery());

            allEntities.Should().HaveCount(entityCount);
        }
    }
}
