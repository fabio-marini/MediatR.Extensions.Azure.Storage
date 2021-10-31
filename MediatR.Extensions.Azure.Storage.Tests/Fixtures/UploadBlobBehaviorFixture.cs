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

namespace MediatR.Extensions.Azure.Storage.Tests
{
    public class UploadBlobBehaviorFixture<TRequest> : UploadBlobBehaviorFixture<TRequest, Unit> where TRequest : IRequest<Unit>
    {
    }

    public class UploadBlobBehaviorFixture<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider svc;
        private readonly Mock<UploadBlobOptions<TRequest>> opt;
        private readonly IMediator med;
        private readonly Mock<BlobClient> blb;
        private readonly Mock<ILogger> log;

        public UploadBlobBehaviorFixture()
        {
            opt = new Mock<UploadBlobOptions<TRequest>>();
            blb = new Mock<BlobClient>("UseDevelopmentStorage=true", "container1", "blob1.txt");
            log = new Mock<ILogger>();

            svc = new ServiceCollection()

                .AddMediatR(this.GetType())

                .AddTransient<UploadBlobCommand<TRequest>>()

                .AddTransient<IPipelineBehavior<TRequest, TResponse>, UploadRequestBehavior<TRequest, TResponse>>()

                .AddTransient<IOptions<UploadBlobOptions<TRequest>>>(sp => Options.Create(opt.Object))

                .AddTransient<PipelineContext>()

                .AddTransient<ILogger>(sp => log.Object)

                .BuildServiceProvider();

            med = svc.GetRequiredService<IMediator>();
        }

        public async Task<TResponse> Test1(TRequest req)
        {
            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Never);
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);

            return res;
        }

        public async Task<TResponse> Test2(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Never);
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);
            opt.VerifyGet(m => m.Metadata, Times.Never);

            return res;
        }

        public async Task<TResponse> Test3(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(3));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Once);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TRequest, PipelineContext, BlobHttpHeaders>>(), Times.Once);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            return res;
        }

        public async Task<TResponse> Test4(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, null);
            opt.SetupProperty(m => m.Metadata, null);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(1));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TRequest, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            return res;
        }

        public async Task<TResponse> Test5(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, null);
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, null);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(3));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Once);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TRequest, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            return res;
        }

        public async Task<TResponse> Test6(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, null);

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Once);
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(2));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TRequest, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            return res;
        }

        public async Task<TResponse> Test7(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, (req, ctx) => new Dictionary<string, string> { { "Powered-By", "MediatR" } });

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Exactly(2));

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TRequest, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Once);

            return res;
        }

        public async Task<TResponse> Test8(TRequest req)
        {
            opt.SetupProperty(m => m.IsEnabled, true);
            opt.SetupProperty(m => m.BlobClient, (req, ctx) => blb.Object);
            opt.SetupProperty(m => m.BlobContent, (req, ctx) => BinaryData.FromString("Hello world! :)"));
            opt.SetupProperty(m => m.BlobHeaders, (req, ctx) => new BlobHttpHeaders { CacheControl = "no-cache" });
            opt.SetupProperty(m => m.Metadata, (req, ctx) => new Dictionary<string, string> { { "Powered-By", "MediatR" } });

            blb.Setup(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None)).Throws(new Exception("Failed! :("));

            var res = await med.Send(req);

            opt.VerifyGet(m => m.IsEnabled, Times.Once);
            opt.VerifyGet(m => m.BlobClient, Times.Exactly(2));
            opt.VerifyGet(m => m.Metadata, Times.Exactly(0));
            opt.VerifyGet(m => m.BlobContent, Times.Exactly(2));
            opt.VerifyGet(m => m.BlobHeaders, Times.Never);

            opt.VerifySet(m => m.BlobContent = It.IsAny<Func<TRequest, PipelineContext, BinaryData>>(), Times.Never);
            opt.VerifySet(m => m.BlobHeaders = It.IsAny<Func<TRequest, PipelineContext, BlobHttpHeaders>>(), Times.Never);

            blb.Verify(m => m.UploadAsync(It.IsAny<BinaryData>(), CancellationToken.None), Times.Once);
            blb.Verify(m => m.SetHttpHeadersAsync(It.IsAny<BlobHttpHeaders>(), null, CancellationToken.None), Times.Never);
            blb.Verify(m => m.SetMetadataAsync(It.IsAny<IDictionary<string, string>>(), null, CancellationToken.None), Times.Never);

            var logInvocation = log.Invocations.Where(i => i.Method.Name == "Log").Single();

            logInvocation.Arguments.OfType<LogLevel>().Single().Should().Be(LogLevel.Error);
            logInvocation.Arguments.OfType<Exception>().Single().Message.Should().Be("Failed! :(");

            return res;
        }
    }
}
