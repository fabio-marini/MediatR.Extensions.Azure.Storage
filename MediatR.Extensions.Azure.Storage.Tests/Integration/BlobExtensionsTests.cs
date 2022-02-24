using Azure.Storage.Blobs;
using FluentAssertions;
using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Tests")]
    public class BlobExtensionsTests : IClassFixture<BlobFixture>
    {
        private readonly BlobFixture blobFixture;
        private readonly ITestOutputHelper log;

        public BlobExtensionsTests(BlobFixture blobFixture, ITestOutputHelper log)
        {
            this.blobFixture = blobFixture;
            this.log = log;
        }

        [Fact(DisplayName = "01. Container is empty")]
        public void Test1() => blobFixture.GivenContainerIsEmpty();

        [Fact(DisplayName = "02. Blobs are uploaded")]
        public async Task Test2()
        {
            var serviceProvider = new ServiceCollection()
                
                .AddMediatR(this.GetType())
                .AddTransient<BlobContainerClient>(sp => blobFixture.Container)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddBlobOptions()
                .AddUploadBlobExtensions()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);
        }

        [Fact(DisplayName = "03. Container has blobs")]
        public void Test3() => blobFixture.ThenContainerHasBlobs(4);

        [Fact(DisplayName = "04. Blobs are downloaded")]
        public async Task Test4()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<BlobContainerClient>(sp => blobFixture.Container)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddBlobOptions()
                .AddDownloadBlobExtensions()
                .AddSingleton<PipelineContext>()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);

            var ctx = serviceProvider.GetRequiredService<PipelineContext>();

            ctx.Should().HaveCount(4);
        }

        [Fact(DisplayName = "05. Blobs are deleted")]
        public async Task Test5()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<BlobContainerClient>(sp => blobFixture.Container)
                .AddTransient<ITestOutputHelper>(sp => log)
                .AddBlobOptions()
                .AddDeleteBlobExtensions()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(EchoRequest.Default);

            res.Message.Should().Be(EchoRequest.Default.Message);
        }

        [Fact(DisplayName = "06. Container is empty")]
        public void Test6() => blobFixture.ThenContainerIsEmpty();
    }
}
