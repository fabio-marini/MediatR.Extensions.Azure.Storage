using Azure.Storage.Blobs;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    public class BlobFixture
    {
        public BlobFixture()
        {
            ContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", "integration-tests");
            ContainerClient.CreateIfNotExists();
        }

        public BlobContainerClient ContainerClient { get; }
    }
}
