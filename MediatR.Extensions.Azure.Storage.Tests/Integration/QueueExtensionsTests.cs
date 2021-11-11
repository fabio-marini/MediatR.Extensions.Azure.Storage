using Azure.Storage.Queues.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Integration
{
    [Trait("TestCategory", "Integration")]
    public class QueueExtensionsTests : IClassFixture<QueueFixture>
    {
        private readonly QueueFixture queueFixture;

        public QueueExtensionsTests(QueueFixture queueFixture)
        {
            this.queueFixture = queueFixture;
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { TestCommand.Default, Unit.Value };
            yield return new object[] { TestQuery.Default, TestResult.Default };
        }

        [Theory(DisplayName = "All QueueRequestBehaviors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test1<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            QueueMessage msg = default;

            var opt = new QueueOptions<TRequest>
            {
                IsEnabled = true,
                QueueClient = queueFixture.QueueClient,
                Received = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TRequest>(res.Body.ToString());

                    obj.Should().NotBeNull();

                    msg = res;

                    return Task.CompletedTask;
                },
                Delete = (ctx, req) => Task.FromResult(msg)
            };

            var serviceProvider = new ServiceCollection()

                .AddQueueExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var sendExtension = serviceProvider.GetRequiredService<SendMessageRequestBehavior<TRequest, TResponse>>();
            var receiveExtension = serviceProvider.GetRequiredService<ReceiveMessageRequestBehavior<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteMessageRequestBehavior<TRequest, TResponse>>();

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);

            _ = await sendExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(1);

            _ = await receiveExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            _ = await deleteExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);
        }

        [Theory(DisplayName = "All QueueResponseBehaviors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test2<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            QueueMessage msg = default;

            var opt = new QueueOptions<TResponse>
            {
                IsEnabled = true,
                QueueClient = queueFixture.QueueClient,
                Received = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TResponse>(res.Body.ToString());

                    obj.Should().NotBeNull();

                    msg = res;

                    return Task.CompletedTask;
                },
                Delete = (ctx, req) => Task.FromResult(msg)
            };

            var serviceProvider = new ServiceCollection()

                .AddQueueExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var sendExtension = serviceProvider.GetRequiredService<SendMessageResponseBehavior<TRequest, TResponse>>();
            var receiveExtension = serviceProvider.GetRequiredService<ReceiveMessageResponseBehavior<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteMessageResponseBehavior<TRequest, TResponse>>();

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);

            _ = await sendExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(1);

            _ = await receiveExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            _ = await deleteExtension.Handle(req, CancellationToken.None, () => Task.FromResult(res));

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);
        }

        [Theory(DisplayName = "All QueueRequestProcessors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test3<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            QueueMessage msg = default;

            var opt = new QueueOptions<TRequest>
            {
                IsEnabled = true,
                QueueClient = queueFixture.QueueClient,
                Received = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TRequest>(res.Body.ToString());

                    obj.Should().NotBeNull();

                    msg = res;

                    return Task.CompletedTask;
                },
                Delete = (ctx, req) => Task.FromResult(msg)
            };

            var serviceProvider = new ServiceCollection()

                .AddQueueExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var sendExtension = serviceProvider.GetRequiredService<SendMessageRequestProcessor<TRequest>>();
            var receiveExtension = serviceProvider.GetRequiredService<ReceiveMessageRequestProcessor<TRequest>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteMessageRequestProcessor<TRequest>>();

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);

            await sendExtension.Process(req, CancellationToken.None);

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(1);

            await receiveExtension.Process(req, CancellationToken.None);

            await deleteExtension.Process(req, CancellationToken.None);

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);
        }

        [Theory(DisplayName = "All QueueResponseProcessors execute correctly"), MemberData(nameof(TestData))]
        public async Task Test4<TRequest, TResponse>(TRequest req, TResponse res) where TRequest : IRequest<TResponse>
        {
            QueueMessage msg = default;

            var opt = new QueueOptions<TResponse>
            {
                IsEnabled = true,
                QueueClient = queueFixture.QueueClient,
                Received = (res, req, ctx) =>
                {
                    var obj = JsonConvert.DeserializeObject<TResponse>(res.Body.ToString());

                    obj.Should().NotBeNull();

                    msg = res;

                    return Task.CompletedTask;
                },
                Delete = (ctx, req) => Task.FromResult(msg)
            };

            var serviceProvider = new ServiceCollection()

                .AddQueueExtensions<TRequest, TResponse>(sp => Options.Create(opt))
                .BuildServiceProvider();

            var sendExtension = serviceProvider.GetRequiredService<SendMessageResponseProcessor<TRequest, TResponse>>();
            var receiveExtension = serviceProvider.GetRequiredService<ReceiveMessageResponseProcessor<TRequest, TResponse>>();
            var deleteExtension = serviceProvider.GetRequiredService<DeleteMessageResponseProcessor<TRequest, TResponse>>();

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);

            await sendExtension.Process(req, res, CancellationToken.None);

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(1);

            await receiveExtension.Process(req, res, CancellationToken.None);

            await deleteExtension.Process(req, res, CancellationToken.None);

            opt.QueueClient.PeekMessages().Value.Should().HaveCount(0);
        }
    }
}
