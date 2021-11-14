using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using MediatR.Extensions.Azure.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands.Blobs
{
    public class DeleteBlobCommandTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<BlobOptions<TestMessage>> opt;
        private readonly Mock<BlobClient> blb;

        private readonly DeleteBlobCommand<TestMessage> cmd;

        public DeleteBlobCommandTests()
        {
            opt = new Mock<BlobOptions<TestMessage>>();
            blb = new Mock<BlobClient>("UseDevelopmentStorage=true", "container1", "blob1.txt");

            svc = new ServiceCollection()

                .AddTransient<DeleteBlobCommand<TestMessage>>()
                .AddTransient<IOptions<BlobOptions<TestMessage>>>(sp => Options.Create(opt.Object))

                .BuildServiceProvider();

            cmd = svc.GetRequiredService<DeleteBlobCommand<TestMessage>>();
        }

        [Fact(DisplayName = "Command is disabled")]
        public async Task Test1()
        {
            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Never);
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Command is cancelled")]
        public async Task Test2()
        {
            var src = new CancellationTokenSource(0);

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, src.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Never);
        }

        [Fact(DisplayName = "BlobClient is not specified")]
        public async Task Test3()
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

        [Fact(DisplayName = "Command throws CommandException")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);

            blb.Setup(m => m.DeleteAsync(DeleteSnapshotsOption.None, null, CancellationToken.None)).ThrowsAsync(new ArgumentNullException());

            Func<Task> act = async () => await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            await act.Should().ThrowAsync<CommandException>();

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Command completes successfully")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);

            var res = new Mock<Response>();
            res.SetupGet(r => r.Status).Returns(200);

            blb.Setup(m => m.DeleteAsync(DeleteSnapshotsOption.None, null, CancellationToken.None)).ReturnsAsync(res.Object);

            await cmd.ExecuteAsync(TestMessage.Default, CancellationToken.None);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);
        }
    }
}
