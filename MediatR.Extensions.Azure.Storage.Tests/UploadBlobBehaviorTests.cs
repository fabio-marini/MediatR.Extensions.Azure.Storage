using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class UploadBlobBehaviorTests
    {
        private readonly IServiceProvider svc;
        private readonly Mock<UploadBlobOptions<TestCommand>> opt;
        private readonly IMediator med;
        private readonly Mock<BlobContainerClient> blb;
        private readonly Mock<ILogger> log;

        public UploadBlobBehaviorTests()
        {
            opt = new Mock<UploadBlobOptions<TestCommand>>();
            blb = new Mock<BlobContainerClient>("UseDevelopmentStorage=true", "container1");
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddMediatR(this.GetType())

                .AddTransient<IPipelineBehavior<TestCommand, Unit>, UploadBlobBehavior<TestCommand>>()

                .AddTransient<IOptions<UploadBlobOptions<TestCommand>>>(sp => Options.Create(opt.Object))

                .AddTransient<PipelineContext>()

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();

            med = svc.GetRequiredService<IMediator>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1()
        {
            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.Container, Times.Never);
            opt.Verify(m => m.BlobContent, Times.Never);
            opt.Verify(m => m.BlobHeaders, Times.Never);
            opt.Verify(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Container is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.Container, Times.Once);
            opt.Verify(m => m.BlobContent, Times.Never);
            opt.Verify(m => m.BlobHeaders, Times.Never);
            opt.Verify(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Behavior uses default BlobContent, BlobName and BlobHeaders")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.Container, blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobName, null);
            opt.SetupProperty(m => m.BlobHeaders, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.Container, Times.Exactly(2));
            opt.Verify(m => m.Container.UploadBlobAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);

            opt.Invocations.Where(i => i.Method.Name == "get_QueueMessage").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_QueueMessage").Should().HaveCount(1);
        }
    }
}
