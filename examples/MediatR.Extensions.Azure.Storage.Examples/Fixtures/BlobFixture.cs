using Azure.Storage.Blobs;
using FluentAssertions;
using Polly;
using System;
using System.Linq;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public class BlobFixture
    {
        private readonly BlobContainerClient blb;
        private readonly ITestOutputHelper log;

        public BlobFixture(BlobContainerClient blb, ITestOutputHelper log)
        {
            this.blb = blb;
            this.log = log;
        }

        public void GivenContainerIsEmpty()
        {
            var allBlobs = blb.GetBlobs();

            if (allBlobs.Any() == false)
            {
                return;
            }

            foreach (var b in allBlobs)
            {
                var res = blb.DeleteBlob(b.Name);

                res.Status.Should().Be(202);
            }
        }

        public void ThenContainerHasBlobs(int expectedCount)
        {
            var retryPolicy = Policy
                .HandleResult<int>(res => res != expectedCount)
                .WaitAndRetry(5, i => TimeSpan.FromSeconds(1));

            var actualCount = retryPolicy.Execute(() =>
            {
                var res = blb.GetBlobs().Count();

                log.WriteLine($"Container {blb.Name} has {res} blobs");

                return res;
            });

            actualCount.Should().Be(expectedCount);
        }

        public void ThenContainerIsEmpty() => ThenContainerHasBlobs(0);

    }
}
