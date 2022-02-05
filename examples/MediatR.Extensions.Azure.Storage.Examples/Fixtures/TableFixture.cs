using FluentAssertions;
using Microsoft.Azure.Cosmos.Table;
using Polly;
using System;
using System.Linq;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public class TableFixture
    {
        private readonly CloudTable tbl;
        private readonly ITestOutputHelper log;

        public TableFixture(CloudTable tbl, ITestOutputHelper log)
        {
            this.tbl = tbl;
            this.log = log;
        }

        public void GivenTableIsEmpty()
        {
            var allEntities = tbl.ExecuteQuery(new TableQuery());

            if (allEntities.Any() == false)
            {
                return;
            }

            foreach (var e in allEntities)
            {
                var res = tbl.Execute(TableOperation.Delete(e));

                res.HttpStatusCode.Should().Be(204);
            }
        }

        public void ThenTableHasEntities(int expectedCount)
        {
            var retryPolicy = Policy
                .HandleResult<int>(res => res != expectedCount)
                .WaitAndRetry(5, i => TimeSpan.FromSeconds(1));

            var actualCount = retryPolicy.Execute(() =>
            {
                var res = tbl.ExecuteQuery(new TableQuery()).Count();

                log.WriteLine($"Table {tbl.Name} has {res} entities");

                return res;
            });

            actualCount.Should().Be(expectedCount);
        }

        public void ThenTableIsEmpty() => ThenTableHasEntities(0);

    }
}
