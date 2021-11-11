using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    public class BlobExtensionsTests : IClassFixture<BlobFixture>
    {
        private readonly BlobFixture blobFixture;

        public BlobExtensionsTests(BlobFixture blobFixture)
        {
            this.blobFixture = blobFixture;
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, Unit.Value };
            yield return new object[] { TestQuery.Default, TestResult.Default };
        }

        [Theory(DisplayName = "All BlobRequestBehaviors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var blobName = Guid.NewGuid().ToString() + ".json";

            var opt = new BlobOptions<TRequest>
            {
                IsEnabled = true,
                BlobClient = (req, ctx) => blobFixture.ContainerClient.GetBlobClient(blobName),
                Downloaded = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TRequest>(res.Content.ToString());

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddBlobExtensions(opt, new BlobOptions<TResponse>())
                .BuildServiceProvider();

            var uploadExtension = serviceProvider.GetRequiredService<UploadBlobRequestBehavior<TRequest, TResponse>>();
            var downloadExtension = serviceProvider.GetRequiredService<DownloadBlobRequestBehavior<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteBlobRequestBehavior<TRequest, TResponse>>();

            opt.BlobClient(req, null).Exists().Value.Should().BeFalse();

            _ = await uploadExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.BlobClient(req, null).Exists().Value.Should().BeTrue();

            _ = await downloadExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            _ = await deleteExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.BlobClient(req, null).Exists().Value.Should().BeFalse();
        }

        [Theory(DisplayName = "All BlobResponseBehaviors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var blobName = Guid.NewGuid().ToString() + ".json";

            var opt = new BlobOptions<TResponse>
            {
                IsEnabled = true,
                BlobClient = (req, ctx) => blobFixture.ContainerClient.GetBlobClient(blobName),
                Downloaded = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TResponse>(res.Content.ToString());

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddBlobExtensions(new BlobOptions<TRequest>(), opt)
                .BuildServiceProvider();

            var uploadExtension = serviceProvider.GetRequiredService<UploadBlobResponseBehavior<TRequest, TResponse>>();
            var downloadExtension = serviceProvider.GetRequiredService<DownloadBlobResponseBehavior<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteBlobResponseBehavior<TRequest, TResponse>>();

            opt.BlobClient(res, null).Exists().Value.Should().BeFalse();

            _ = await uploadExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.BlobClient(res, null).Exists().Value.Should().BeTrue();

            _ = await downloadExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            _ = await deleteExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.BlobClient(res, null).Exists().Value.Should().BeFalse();
        }

        [Theory(DisplayName = "All BlobRequestProcessors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test3<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var blobName = Guid.NewGuid().ToString() + ".json";

            var opt = new BlobOptions<TRequest>
            {
                IsEnabled = true,
                BlobClient = (req, ctx) => blobFixture.ContainerClient.GetBlobClient(blobName),
                Downloaded = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TRequest>(res.Content.ToString());

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddBlobExtensions(opt, new BlobOptions<TResponse>())
                .BuildServiceProvider();

            var uploadExtension = serviceProvider.GetRequiredService<UploadBlobRequestProcessor<TRequest>>();
            var downloadExtension = serviceProvider.GetRequiredService<DownloadBlobRequestProcessor<TRequest>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteBlobRequestProcessor<TRequest>>();

            opt.BlobClient(req, null).Exists().Value.Should().BeFalse();

            await uploadExtension.Process(req, CancellationToken.None);

            opt.BlobClient(req, null).Exists().Value.Should().BeTrue();

            await downloadExtension.Process(req, CancellationToken.None);

            await deleteExtension.Process(req, CancellationToken.None);

            opt.BlobClient(req, null).Exists().Value.Should().BeFalse();
        }

        [Theory(DisplayName = "All BlobResponseProcessors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test4<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            var blobName = Guid.NewGuid().ToString() + ".json";

            var opt = new BlobOptions<TResponse>
            {
                IsEnabled = true,
                BlobClient = (req, ctx) => blobFixture.ContainerClient.GetBlobClient(blobName),
                Downloaded = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TResponse>(res.Content.ToString());

                    obj.Should().NotBeNull();

                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()

                .AddBlobExtensions(new BlobOptions<TRequest>(), opt)
                .BuildServiceProvider();

            var uploadExtension = serviceProvider.GetRequiredService<UploadBlobResponseProcessor<TRequest, TResponse>>();
            var downloadExtension = serviceProvider.GetRequiredService<DownloadBlobResponseProcessor<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteBlobResponseProcessor<TRequest, TResponse>>();

            opt.BlobClient(res, null).Exists().Value.Should().BeFalse();

            await uploadExtension.Process(req, res, CancellationToken.None);

            opt.BlobClient(res, null).Exists().Value.Should().BeTrue();

            await downloadExtension.Process(req, res, CancellationToken.None);

            await deleteExtension.Process(req, res, CancellationToken.None);

            opt.BlobClient(res, null).Exists().Value.Should().BeFalse();
        }
    }
}
