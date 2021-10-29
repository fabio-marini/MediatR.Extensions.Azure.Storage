using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
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
        private readonly Mock<BlobClient> blb;
        private readonly Mock<ILogger> log;

        public UploadBlobBehaviorTests()
        {
            opt = new Mock<UploadBlobOptions<TestCommand>>();
            blb = new Mock<BlobClient>("UseDevelopmentStorage=true", "container1", "blob1.txt");
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
            opt.Verify(m => m.BlobClient, Times.Never);
            opt.Verify(m => m.BlobContent, Times.Never);
            opt.Verify(m => m.BlobHeaders, Times.Never);
            opt.Verify(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "BlobClient is not specified")]
        public async Task Test2()
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Once);
            opt.Verify(m => m.BlobContent, Times.Never);
            opt.Verify(m => m.BlobHeaders, Times.Never);
            opt.Verify(m => m.Metadata, Times.Never);
        }

        [Fact(DisplayName = "Behavior uses default BlobContent and BlobHeaders")]
        public async Task Test3()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Exactly(2));
            opt.Verify(m => m.Metadata, Times.Once);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobContent").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobContent").Should().HaveCount(1);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobHeaders").Should().HaveCount(3);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobHeaders").Should().HaveCount(1);
        }

        [Fact(DisplayName = "Behavior uses specified BlobContent")]
        public async Task Test4()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Exactly(2));
            opt.Verify(m => m.Metadata, Times.Once);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobContent").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobContent").Should().HaveCount(0);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobHeaders").Should().HaveCount(1);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobHeaders").Should().HaveCount(0);
        }

        [Fact(DisplayName = "Behavior uses specified BlobHeaders")]
        public async Task Test5()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Exactly(2));
            opt.Verify(m => m.Metadata, Times.Once);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobContent").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobContent").Should().HaveCount(1);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobHeaders").Should().HaveCount(3);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobHeaders").Should().HaveCount(0);
        }

        [Fact(DisplayName = "Behavior uses specified BlobContent and BlobHeaders")]
        public async Task Test6()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, null);

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Exactly(2));
            opt.Verify(m => m.Metadata, Times.Once);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobContent").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobContent").Should().HaveCount(0);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobHeaders").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobHeaders").Should().HaveCount(0);
        }

        [Fact(DisplayName = "Behavior uses specified Metadata")]
        public async Task Test7()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, (req, ctx) => new Dictionary<string, string> { { "Powered-By", "MediatR" } });

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Exactly(2));
            opt.Verify(m => m.Metadata, Times.Exactly(2));

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Once);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobContent").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobContent").Should().HaveCount(0);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobHeaders").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobHeaders").Should().HaveCount(0);
        }

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test8()
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, (req, ctx) => new Dictionary<string, string> { { "Powered-By", "MediatR" } });

            blb.Setup(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None)).Throws(new Exception("Failed! :("));

            var cmd = new TestCommand { Message = "Hello! :)" };

            _ = await med.Send(cmd);

            opt.Verify(m => m.IsEnabled, Times.Once);
            opt.Verify(m => m.BlobClient, Times.Exactly(2));
            opt.Verify(m => m.Metadata, Times.Exactly(0));

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobContent").Should().HaveCount(2);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobContent").Should().HaveCount(0);

            opt.Invocations.Where(i => i.Method.Name == "get_BlobHeaders").Should().HaveCount(0);
            opt.Invocations.Where(i => i.Method.Name == "set_BlobHeaders").Should().HaveCount(0);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single();
        }

    }
}
