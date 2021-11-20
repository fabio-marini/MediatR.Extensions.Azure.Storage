using Azure.Storage.Blobs;
using FluentAssertions;
using System.Linq;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public class BlobFixture
    {
        public BlobFixture()
        {
            Container = new BlobContainerClient("UseDevelopmentStorage=true", "integration-tests");
            Container.CreateIfNotExists();
        }

        public BlobContainerClient Container { get; }

        public void GivenContainerIsEmpty()
        {
            var allBlobs = Container.GetBlobs();

            if (allBlobs.Any() == false)
            {
                return;
            }

            foreach (var b in allBlobs)
            {
                var res = Container.DeleteBlob(b.Name);

                res.Status.Should().Be(202);
            }
        }

        public void ThenContainerIsEmpty()
        {
            var allBlobs = Container.GetBlobs();

            allBlobs.Any().Should().BeFalse();
        }

        public void ThenContainerHasBlobs(int blobCount)
        {
            var allBlobs = Container.GetBlobs();

            allBlobs.Should().HaveCount(blobCount);
        }
    }
}
