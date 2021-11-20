using Azure.Storage.Blobs;
using FluentAssertions;
using MediatR.Extensions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    [TestCaseOrderer("MediatR.Extensions.Tests.TestMethodNameOrderer", "MediatR.Extensions.Azure.Storage.Tests")]
    public class BlobExtensionsTests : IClassFixture<BlobFixture>
    {
        private readonly BlobFixture blobFixture;

        public BlobExtensionsTests(BlobFixture blobFixture)
        {
            this.blobFixture = blobFixture;
        }

        [Fact(DisplayName = "01. Container is empty")]
        public void Test1() => blobFixture.GivenContainerIsEmpty();

        [Fact(DisplayName = "02. Blobs are uploaded")]
        public async Task Test2()
        {
            var serviceProvider = new ServiceCollection()
                
                .AddMediatR(this.GetType())
                .AddTransient<BlobContainerClient>(sp => blobFixture.Container)
                .AddBlobOptions<TestQuery, TestResult>()
                .AddUploadBlobExtensions<TestQuery, TestResult>()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(TestQuery.Default);

            res.Length.Should().Be(TestQuery.Default.Message.Length);
        }

        [Fact(DisplayName = "03. Container has blobs")]
        public void Test3() => blobFixture.ThenContainerHasBlobs(4);

        [Fact(DisplayName = "04. Blobs are downloaded")]
        public async Task Test4()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<BlobContainerClient>(sp => blobFixture.Container)
                .AddBlobOptions<TestQuery, TestResult>()
                .AddDownloadBlobExtensions<TestQuery, TestResult>()
                .AddSingleton<PipelineContext>()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(TestQuery.Default);

            res.Length.Should().Be(TestQuery.Default.Message.Length);

            var ctx = serviceProvider.GetRequiredService<PipelineContext>();

            ctx.Should().HaveCount(4);
        }

        [Fact(DisplayName = "05. Blobs are deleted")]
        public async Task Test5()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(this.GetType())
                .AddTransient<BlobContainerClient>(sp => blobFixture.Container)
                .AddBlobOptions<TestQuery, TestResult>()
                .AddDeleteBlobExtensions<TestQuery, TestResult>()

                .BuildServiceProvider();

            var med = serviceProvider.GetRequiredService<IMediator>();

            var res = await med.Send(TestQuery.Default);

            res.Length.Should().Be(TestQuery.Default.Message.Length);
        }

        [Fact(DisplayName = "06. Container is empty")]
        public void Test6() => blobFixture.ThenContainerIsEmpty();
    }
}
