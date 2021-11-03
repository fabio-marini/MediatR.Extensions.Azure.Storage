using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands
{
    public class UploadBlobCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<UploadBlobOptions<TestMessage>> opt;
        private readonly Mock<BlobClient> blb;

        private readonly UploadBlobCommand<TestMessage> cmd;

        public UploadBlobCommandTests()
        {
            opt = new Mock<UploadBlobOptions<TestMessage>>();
            blb = new Mock<BlobClient>("UseDevelopmentStorage=true", "container1", "blob1.txt");

            svc = new ServiceCollection()

                .AddTransient<UploadBlobCommand<TestMessage>>()
                .AddTransient<IOptions<UploadBlobOptions<TestMessage>>>(sp => Options.Create(opt.Object))

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<UploadBlobCommand<TestMessage>>();
        }

        [Fact(DisplayName = "Command is disabled")]
        public async Task Test1a()
        {
            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Never);
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Command is cancelled")]
        public async Task Test1b()
        {
            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, src.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Never);
        }

        [Fact(DisplayName = "BlobClient is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Command uses default BlobContent and BlobHeaders")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(3));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Once);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TestMessage, PipelineContext, BlobHttpHeaders>>(), Times.Once);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "BlobContent delegate returns null")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => null);
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Never);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TestMessage, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Command uses specified BlobContent")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(1));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TestMessage, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Command uses specified BlobHeaders")]
        public async Task Test6()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, null);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(3));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Once);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TestMessage, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Command uses specified BlobContent and BlobHeaders")]
        public async Task Test7()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, null);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(2));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TestMessage, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Command uses specified Metadata")]
        public async Task Test8()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, (req, ctx) => new Dictionary<string, string> { { "Powered-By", "MediatR" } });

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(2));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TestMessage, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TestMessage, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Once);
        }
    }
}
